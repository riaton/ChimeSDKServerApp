using Xunit;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;
using ChimeApp.LambdaFunctions;
using ChimeApp.Domain;
using Moq;
using Newtonsoft.Json;

namespace ChimeSDKServerApp.Tests.LambdaFunctionTests
{
    public class GetMeetingFunctionTest
    {
        private readonly Mock<IDynamoDBRepository> _dynamoDBMock;
        public GetMeetingFunctionTest()
        {
            _dynamoDBMock = new Mock<IDynamoDBRepository>();

            _dynamoDBMock.Setup(x => x.GetMeetingInfo("externalMeetingId"))
            .Callback<string>((request) =>
            {
                request.Is("externalMeetingId");
            })
            .ReturnsAsync("meetingInfo");
        }

        [Fact]
        [Trait("Category", "LambdaFunction")]
        public async Task ミーティング取得ロジックのテスト()
        {
            var function = new GetMeetingFunction(_dynamoDBMock.Object);
            var context = new TestLambdaContext();
            var req = new APIGatewayProxyRequest();
            req.PathParameters = new Dictionary<string, string> { { "externalMeetingId", "externalMeetingId" } };

            var resBody = new GetMeetingResponseT();
            resBody.MeetingInfo = "meetingInfo";

            var res = await function.GetMeetingFromDB(req, context);

            res.StatusCode.Is(CommonResult.OK);
            res.Headers.Is(CommonResult.ResponseHeader);
            res.Body.Is(JsonConvert.SerializeObject(resBody));

            _dynamoDBMock.Verify(x => x.GetMeetingInfo("externalMeetingId"), Times.Once);

            req.PathParameters["externalMeetingId"] = string.Empty;

            res = await function.GetMeetingFromDB(req, context);

            res.StatusCode.Is(CommonResult.ValidateError);
            res.Headers.Is(CommonResult.ResponseHeader);
            res.Body.Is(string.Empty);
            _dynamoDBMock.Verify(x => x.GetMeetingInfo("externalMeetingId"), Times.Once);

            req.PathParameters["externalMeetingId"] = "externalMeetingId";

            _dynamoDBMock.Setup(x => x.GetMeetingInfo("externalMeetingId"))
                .ThrowsAsync(new EnvironmentVariableException("bad environment variable"));

            res = await function.GetMeetingFromDB(req, context);

            res.StatusCode.Is(CommonResult.InternalServerError);
            res.Headers.Is(CommonResult.ResponseHeader);
            res.Body.Is(string.Empty);
            _dynamoDBMock.Verify(x => x.GetMeetingInfo("externalMeetingId"), Times.Exactly(2));

            _dynamoDBMock.Setup(x => x.GetMeetingInfo("externalMeetingId"))
                .ThrowsAsync(new Exception("exception"));

            res = await function.GetMeetingFromDB(req, context);

            res.StatusCode.Is(CommonResult.InternalServerError);
            res.Headers.Is(CommonResult.ResponseHeader);
            res.Body.Is(string.Empty);
            _dynamoDBMock.Verify(x => x.GetMeetingInfo("externalMeetingId"), Times.Exactly(3));
        }
    }

    public class GetMeetingResponseT
    {
        public string? MeetingInfo { get; set; }
    }
}
