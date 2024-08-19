using Amazon.Lambda.APIGatewayEvents;
using Google.Protobuf;
using Newtonsoft.Json;
using ChimeApp.Domain;

namespace ChimeApp.Factories
{
    public class ResponseFactory
    {
        public static APIGatewayProxyResponse CreateResponse(int statusCode, IMessage? body = null)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = statusCode,
                Headers = CommonResult.ResponseHeader,
                Body = body == null ? string.Empty : JsonConvert.SerializeObject(body)
            };
        }
    }
}
