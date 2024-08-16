using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using ChimeApp.Domain;
using ChimeApp.Factories;
using ChimeApp.Models;
using ChimeApp.Infrastructure;
using Amazon.ChimeSDKMeetings;
using Amazon.DynamoDBv2;

//[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ChimeApp.LambdaFunctions;

public class LeaveMeetingFunction
{
    private readonly IMeetingOperation _meetingRepository;
    private readonly IDynamoDBOperation _dynamoDBRepository;
    public LeaveMeetingFunction()
    {
        _meetingRepository = new MeetingOperation(new AmazonChimeSDKMeetingsClient());
        _dynamoDBRepository = new DynamoDBOperation(new AmazonDynamoDBClient());
    }

    /// <summary>
    /// Amazon Chime SDKを用いて指定ミーティングの指定参加者を削除する
    /// </summary>
    /// <param name="input">API Gatewayからの要求</param>
    /// <param name="context">Lambda環境のログ記録と記述のためのメソッド</param>
    /// <returns>APIGatewayProxyResponse</returns>
    public async Task<APIGatewayProxyResponse> LeaveMeeting(APIGatewayProxyRequest input, ILambdaContext context)
    {
        try
        {
            //デシリアライズとバリデーション
            LeaveMeetingRequest message = LeaveMeetingRequest.Parser.ParseJson(input.Body);
            var (validateOk, model) = ModelFactory.CreateModel(message);
            if (validateOk == false || model == null)
            {
                return ResponseFactory.CreateResponse(CommonResult.ValidateError);
            }
            //参加者の削除
            await _meetingRepository.LeaveMeeting(message.MeetingId, message.AttendeeId);
            //DynamoDBから参加者情報を削除
            await _dynamoDBRepository.DeleteAttendeeInfo(message.AttendeeId);

            //クライアントに返却
            return ResponseFactory.CreateResponse(CommonResult.OK);
        }
        catch(Exception e)
        {
            context.Logger.LogLine(e.Message);
            return ResponseFactory.CreateResponse(CommonResult.InternalServerError);
        }
    }
}
