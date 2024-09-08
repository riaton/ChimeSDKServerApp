using Xunit;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;
using ChimeApp.LambdaFunctions;
using ChimeApp.Domain;
using Moq;
using Newtonsoft.Json;

namespace ChimeSDKServerApp.Tests.LambdaFunctionTests
{
    public class GetAttendeeFunctionTest
    {
        private readonly Mock<IDynamoDBRepository> _dynamoDBMock;
        public GetAttendeeFunctionTest()
        {
            _dynamoDBMock = new Mock<IDynamoDBRepository>();

            _dynamoDBMock.Setup(x => x.GetAttendeeInfo("attendeeId"))
            .Callback<string>((request) =>
            {
                request.Is("attendeeId");
            })
            .ReturnsAsync("externalAttendeeId");
        }

        [Fact]
        [Trait("Category", "LambdaFunction")]
        public async Task ミーティング参加者取得ロジックのテスト()
        {
            var function = new GetAttendeeFunction(_dynamoDBMock.Object);
            var context = new TestLambdaContext();
            var req = new APIGatewayProxyRequest();
            req.PathParameters = new Dictionary<string, string> { { "attendeeId", "attendeeId" } };

            var resBody = new GetAttendeeResponseT();
            resBody.ExternalAttendeeId = "externalAttendeeId";

            var res = await function.GetAttendeeFromDB(req, context);

            res.StatusCode.Is(CommonResult.OK);
            res.Headers.Is(CommonResult.ResponseHeader);
            res.Body.Is(JsonConvert.SerializeObject(resBody));

            _dynamoDBMock.Verify(x => x.GetAttendeeInfo("attendeeId"), Times.Once);

            req.PathParameters["attendeeId"] = string.Empty;

            res = await function.GetAttendeeFromDB(req, context);

            res.StatusCode.Is(CommonResult.ValidateError);
            res.Headers.Is(CommonResult.ResponseHeader);
            res.Body.Is(string.Empty);
            _dynamoDBMock.Verify(x => x.GetAttendeeInfo("attendeeId"), Times.Once);

            req.PathParameters["attendeeId"] = "attendeeId";

            _dynamoDBMock.Setup(x => x.GetAttendeeInfo("attendeeId"))
                .ThrowsAsync(new EnvironmentVariableException("bad environment variable"));

            res = await function.GetAttendeeFromDB(req, context);

            res.StatusCode.Is(CommonResult.InternalServerError);
            res.Headers.Is(CommonResult.ResponseHeader);
            res.Body.Is(string.Empty);
            _dynamoDBMock.Verify(x => x.GetAttendeeInfo("attendeeId"), Times.Exactly(2));

            _dynamoDBMock.Setup(x => x.GetAttendeeInfo("attendeeId"))
                .ThrowsAsync(new Exception("exception"));

            res = await function.GetAttendeeFromDB(req, context);

            res.StatusCode.Is(CommonResult.InternalServerError);
            res.Headers.Is(CommonResult.ResponseHeader);
            res.Body.Is(string.Empty);
            _dynamoDBMock.Verify(x => x.GetAttendeeInfo("attendeeId"), Times.Exactly(3));
        }
    }

    public class GetAttendeeResponseT
    {
        public string? ExternalAttendeeId { get; set; }
    }
}
