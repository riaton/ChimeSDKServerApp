using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using ChimeApp.Domain;
using ChimeApp.Factories;
using ChimeApp.Models;
using ChimeApp.Infrastructure;


namespace ChimeApp.LambdaFunctions;

public class GetMeetingFunction
{
    private readonly IDynamoDBOperation _dynamoDBRepository;
    public GetMeetingFunction()
    {
        _dynamoDBRepository = new DynamoDBOperation();
    }

    /// <summary>
    /// DynamoDBから要求のexternalMeetingIdに一致するミーディング情報を取得する
    /// </summary>
    /// <param name="input">API Gatewayからの要求</param>
    /// <param name="context">Lambda環境のログ記録と記述のためのメソッド</param>
    /// <returns>APIGatewayProxyResponse</returns>
    public async Task<APIGatewayProxyResponse> GetMeetingFromDB(APIGatewayProxyRequest input, ILambdaContext context)
    {
        try
        {
            //バリデーション
            if (string.IsNullOrEmpty(input.PathParameters["externalMeetingId"]))
            {
                return ResponseFactory.CreateResponse(CommonResult.ValidateError);
            }
            //DynamoDBからexternalMeetingIdに一致するミーディング情報を取得
            string meetingData = await _dynamoDBRepository.GetMeetingInfo(input.PathParameters["externalMeetingId"]);

            GetMeetingResponse response = new();
            response.MeetingInfo = meetingData;

            //クライアントに返却
            return ResponseFactory.CreateResponse(CommonResult.OK, response);
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
