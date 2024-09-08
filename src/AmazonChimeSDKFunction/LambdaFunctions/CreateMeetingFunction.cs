using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using ChimeApp.Domain;
using ChimeApp.Factories;
using ChimeApp.Models;
using ChimeApp.Infrastructure;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ChimeApp.LambdaFunctions;

public class CreateMeetingFunction
{
    private readonly IMeetingRepository _meetingRepository;
    private readonly IDynamoDBRepository _dynamoDBRepository;
    public CreateMeetingFunction() : this(new MeetingOperation(), new DynamoDBOperation()) { }

    public CreateMeetingFunction(IMeetingRepository meeting, IDynamoDBRepository dynamo)
    {
        _meetingRepository = meeting;
        _dynamoDBRepository = dynamo;
    }

    /// <summary>
    /// Amazon Chime SDKを用いてミーディングを作成し、参加者を追加する
    /// その後作成したミーティング情報と参加者情報をDynamoDBに保存する
    /// 最後にミーティングの情報と参加者の情報をクライアントに返却する
    /// </summary>
    /// <param name="input">API Gatewayからの要求</param>
    /// <param name="context">Lambda環境のログ記録と記述のためのメソッド</param>
    /// <returns>APIGatewayProxyResponse</returns>
    public async Task<APIGatewayProxyResponse> CreateMeeting(APIGatewayProxyRequest input, ILambdaContext context)
    {
        try
        {
            //デシリアライズとバリデーション
            CreateMeetingRequest message = CreateMeetingRequest.Parser.ParseJson(input.Body);
            var (validateOk, model) = ModelFactory.CreateModel(message);
            if (validateOk == false || model == null)
            {
                return ResponseFactory.CreateResponse(CommonResult.ValidateError);
            }
            //ミーティング作成
            var meetingInfo = await _meetingRepository.CreateMeeting(model);
            //ミーティングへの参加
            var joinInfo = await _meetingRepository.JoinMeeting(meetingInfo.MeetingId, message.ExternalAttendeeId);
            //DynamoDBに保存
            await _dynamoDBRepository.RegisterMeetingInfo(meetingInfo);
            await _dynamoDBRepository.RegisterAttendeeInfo(joinInfo);

            CreateMeetingResponse response = new();
            response.MeetingInfo = JsonSerializer.Serialize(meetingInfo);
            response.AttendeeInfo = JsonSerializer.Serialize(joinInfo);

            context.Logger.LogLine("meeting: " + meetingInfo.ToString());
            context.Logger.LogLine("attendee: " + joinInfo.ToString());
            //クライアントに返却
            return ResponseFactory.CreateResponse(CommonResult.OK, response);
        }
        catch(EnvironmentVariableException ex)
        {
            context.Logger.LogLine(ex.Message);
            return ResponseFactory.CreateResponse(CommonResult.InternalServerError);
        }
        catch(Exception e)
        {
            context.Logger.LogLine(e.Message);
            return ResponseFactory.CreateResponse(CommonResult.InternalServerError);
        }
    }
}
