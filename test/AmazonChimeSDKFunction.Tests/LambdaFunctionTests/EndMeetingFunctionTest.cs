using Xunit;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;
using ChimeApp.LambdaFunctions;
using ChimeApp.Domain;
using Moq;
using System.Text.Json;

namespace ChimeSDKServerApp.Tests.LambdaFunctionTests
{
    public class EndMeetingFunctionTest
    {
        private readonly Mock<IMeetingRepository> _meetingMock;
        private readonly Mock<IDynamoDBRepository> _dynamoDBMock;
        public EndMeetingFunctionTest()
        {
            _meetingMock = new Mock<IMeetingRepository>();
            _dynamoDBMock = new Mock<IDynamoDBRepository>();
            _meetingMock.Setup(x => x.EndMeeting("meetingId"))
            .Callback<string>((request) =>
            {
                request.Is("meetingId");
            });

            _dynamoDBMock.Setup(x => x.DeleteMeetingInfo("externalMeetingId"))
            .Callback<string>((request) =>
            {
                request.Is("externalMeetingId");
            });
        }

        [Fact]
        [Trait("Category", "LambdaFunction")]
        public async Task ミーティング終了ロジックのテスト()
        {
            var function = new EndMeetingFunction(_meetingMock.Object, _dynamoDBMock.Object);
            var context = new TestLambdaContext();
            var req = new APIGatewayProxyRequest();
            var reqBody = new EndMeetingRequestT();
            req.Body = JsonSerializer.Serialize(reqBody);

            var res = await function.EndMeeting(req, context);

            res.StatusCode.Is(CommonResult.OK);
            res.Headers.Is(CommonResult.ResponseHeader);
            res.Body.Is(string.Empty);

            _meetingMock.Verify(x => x.EndMeeting("meetingId"), Times.Once);
            _dynamoDBMock.Verify(x => x.DeleteMeetingInfo("externalMeetingId"), Times.Once);

            reqBody.MeetingId = string.Empty;
            req.Body = JsonSerializer.Serialize(reqBody);

            res = await function.EndMeeting(req, context);

            res.StatusCode.Is(CommonResult.ValidateError);
            res.Headers.Is(CommonResult.ResponseHeader);
            res.Body.Is(string.Empty);

            _meetingMock.Verify(x => x.EndMeeting("meetingId"), Times.Once);
            _dynamoDBMock.Verify(x => x.DeleteMeetingInfo("externalMeetingId"), Times.Once);


            _meetingMock.Setup(x => x.EndMeeting("meetingId"))
           .ThrowsAsync(new EnvironmentVariableException("bad environment variable"));

            reqBody.MeetingId = "meetingId";
            req.Body = JsonSerializer.Serialize(reqBody);

            res = await function.EndMeeting(req, context);
            res.StatusCode.Is(CommonResult.InternalServerError);
            res.Headers.Is(CommonResult.ResponseHeader);
            res.Body.Is(string.Empty);

            _meetingMock.Verify(x => x.EndMeeting("meetingId"), Times.Exactly(2));
            _dynamoDBMock.Verify(x => x.DeleteMeetingInfo("externalMeetingId"), Times.Once);

            _meetingMock.Setup(x => x.EndMeeting("meetingId"))
           .ThrowsAsync(new Exception("exception"));

            res = await function.EndMeeting(req, context);
            res.StatusCode.Is(CommonResult.InternalServerError);
            res.Headers.Is(CommonResult.ResponseHeader);
            res.Body.Is(string.Empty);

            _meetingMock.Verify(x => x.EndMeeting("meetingId"), Times.Exactly(3));
            _dynamoDBMock.Verify(x => x.DeleteMeetingInfo("externalMeetingId"), Times.Once);
        }
    }

    public class EndMeetingRequestT
    {
        public string MeetingId { get; set; } = "meetingId";
        public string ExternalMeetingId { get; set; } = "externalMeetingId";
    }
}
