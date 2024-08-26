using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using ChimeApp.Domain;
using ChimeApp.Factories;
using ChimeApp.Models;
using ChimeApp.Infrastructure;


namespace ChimeApp.LambdaFunctions;

public class EndMeetingFunction
{
    private readonly IMeetingRepository _meetingRepository;
    private readonly IDynamoDBRepository _dynamoDBRepository;
    public EndMeetingFunction()
    {
        _meetingRepository = new MeetingOperation();
        _dynamoDBRepository = new DynamoDBOperation();
    }

    /// <summary>
    /// Amazon Chime SDKを用いてミーディングを終了し、参加者を削除する
    /// その後会議情報をDynamoDBから削除する
    /// </summary>
    /// <param name="input">API Gatewayからの要求</param>
    /// <param name="context">Lambda環境のログ記録と記述のためのメソッド</param>
    /// <returns>APIGatewayProxyResponse</returns>
    public async Task<APIGatewayProxyResponse> EndMeeting(APIGatewayProxyRequest input, ILambdaContext context)
    {
        try
        {
            //デシリアライズとバリデーション
            EndMeetingRequest message = EndMeetingRequest.Parser.ParseJson(input.Body);
            var (validateOk, model) = ModelFactory.CreateModel(message);
            if (validateOk == false || model == null)
            {
                return ResponseFactory.CreateResponse(CommonResult.ValidateError);
            }
            //ミーティングの削除
            await _meetingRepository.EndMeeting(message.MeetingId);
            //DynamoDBから会議情報を削除
            await _dynamoDBRepository.DeleteMeetingInfo(message.ExternalMeetingId);

            //クライアントに返却
            return ResponseFactory.CreateResponse(CommonResult.OK);
        }
        catch (EnvironmentVariableException ex)
        {
            context.Logger.LogLine(ex.Message);
            return ResponseFactory.CreateResponse(CommonResult.InternalServerError);
        }
        catch (Exception e)
        {
            context.Logger.LogLine(e.Message);
            return ResponseFactory.CreateResponse(CommonResult.InternalServerError);
        }
    }
}
