using Amazon.Lambda.APIGatewayEvents;
using Google.Protobuf;
using Newtonsoft.Json;

namespace ChimeApp.Factories
{
    internal class ResponseFactory
    {
        internal static APIGatewayProxyResponse CreateResponse(int statusCode, IMessage? body = null)
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
