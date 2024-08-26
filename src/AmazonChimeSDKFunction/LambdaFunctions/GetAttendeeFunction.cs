using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using ChimeApp.Domain;
using ChimeApp.Factories;
using ChimeApp.Models;
using ChimeApp.Infrastructure;


namespace ChimeApp.LambdaFunctions;

public class GetAttendeeFunction
{
    private readonly IDynamoDBRepository _dynamoDBRepository;
    public GetAttendeeFunction()
    {
        _dynamoDBRepository = new DynamoDBOperation();
    }

    /// <summary>
    /// DynamoDBから要求のattendeeIdに一致する参加者情報を取得する
    /// </summary>
    /// <param name="input">API Gatewayからの要求</param>
    /// <param name="context">Lambda環境のログ記録と記述のためのメソッド</param>
    /// <returns>APIGatewayProxyResponse</returns>
    public async Task<APIGatewayProxyResponse> GetAttendeeFromDB(APIGatewayProxyRequest input, ILambdaContext context)
    {
        try
        {
            //バリデーション
            if (string.IsNullOrEmpty(input.PathParameters["attendeeId"]))
            {
                return ResponseFactory.CreateResponse(CommonResult.ValidateError);
            }
            //DynamoDBからAttendeeIdに一致するexternalAttendeeIdを取得
            string externalAttendeeId = await _dynamoDBRepository.GetAttendeeInfo(input.PathParameters["attendeeId"]);

            GetAttendeeResponse response = new();
            response.ExternalAttendeeId = externalAttendeeId;

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
