using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using ChimeApp.Domain;
using ChimeApp.Factories;
using ChimeApp.Models;
using ChimeApp.Infrastructure;
using Amazon.ChimeSDKMeetings;
using System.Text.Json;
using Amazon.DynamoDBv2;

//[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ChimeApp.LambdaFunctions;

public class JoinMeetingFunction
{
    private readonly IMeetingOperation _meetingRepository;
    private readonly IDynamoDBOperation _dynamoDBRepository;
    public JoinMeetingFunction()
    {
        _meetingRepository = new MeetingOperation(new AmazonChimeSDKMeetingsClient());
        _dynamoDBRepository = new DynamoDBOperation(new AmazonDynamoDBClient());
    }

    /// <summary>
    /// Amazon Chime SDKを用いてミーディングに参加者を追加する
    /// その後参加者情報をDynamoDBに保存する
    /// 最後に参加者の情報をクライアントに返却する
    /// </summary>
    /// <param name="input">API Gatewayからの要求</param>
    /// <param name="context">Lambda環境のログ記録と記述のためのメソッド</param>
    /// <returns>APIGatewayProxyResponse</returns>
    public async Task<APIGatewayProxyResponse> JoinMeeting(APIGatewayProxyRequest input, ILambdaContext context)
    {
        try
        {
            //デシリアライズとバリデーション
            JoinMeetingRequest message = JoinMeetingRequest.Parser.ParseJson(input.Body);
            var (validateOk, model) = ModelFactory.CreateModel(message);
            if (validateOk == false || model == null)
            {
                return ResponseFactory.CreateResponse(CommonResult.ValidateError);
            }
            //ミーティングへの参加
            var joinInfo = await _meetingRepository.JoinMeeting(message.MeetingId, message.ExternalAttendeeId);
            //DynamoDBに保存
            await _dynamoDBRepository.RegisterAttendeeInfo(joinInfo);

            JoinMeetingResponse response = new();
            response.AttendeeInfo = JsonSerializer.Serialize(joinInfo);

            //クライアントに返却
            return ResponseFactory.CreateResponse(CommonResult.OK, response);
        }
        catch(Exception e)
        {
            context.Logger.LogLine(e.Message);
            return ResponseFactory.CreateResponse(CommonResult.InternalServerError);
        }
    }
}
