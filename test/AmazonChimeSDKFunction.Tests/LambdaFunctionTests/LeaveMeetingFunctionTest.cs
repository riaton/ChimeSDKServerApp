using Xunit;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;
using ChimeApp.LambdaFunctions;
using ChimeApp.Domain;
using Moq;
using System.Text.Json;

namespace ChimeSDKServerApp.Tests.LambdaFunctionTests
{
    public class LeaveMeetingFunctionTest
    {
        private readonly Mock<IMeetingRepository> _meetingMock;
        private readonly Mock<IDynamoDBRepository> _dynamoDBMock;
        public LeaveMeetingFunctionTest()
        {
            _meetingMock = new Mock<IMeetingRepository>();
            _dynamoDBMock = new Mock<IDynamoDBRepository>();
            _meetingMock.Setup(x => x.LeaveMeeting("meetingId", "attendeeId"))
            .Callback<string, string>((meeting, attendee) =>
            {
                meeting.Is("meetingId");
                attendee.Is("attendeeId");
            });

            _dynamoDBMock.Setup(x => x.DeleteAttendeeInfo("attendeeId"))
            .Callback<string>((request) =>
            {
                request.Is("attendeeId");
            });
        }

        [Fact]
        [Trait("Category", "LambdaFunction")]
        public async Task ミーティング退出ロジックのテスト()
        {
            var function = new LeaveMeetingFunction(_meetingMock.Object, _dynamoDBMock.Object);
            var context = new TestLambdaContext();
            var req = new APIGatewayProxyRequest();
            var reqBody = new LeaveMeetingRequestT();
            req.Body = JsonSerializer.Serialize(reqBody);

            var res = await function.LeaveMeeting(req, context);

            res.StatusCode.Is(CommonResult.OK);
            res.Headers.Is(CommonResult.ResponseHeader);
            res.Body.Is(string.Empty);

            _meetingMock.Verify(x => x.LeaveMeeting("meetingId", "attendeeId"), Times.Once);
            _dynamoDBMock.Verify(x => x.DeleteAttendeeInfo("attendeeId"), Times.Once);

            reqBody.MeetingId = string.Empty;
            req.Body = JsonSerializer.Serialize(reqBody);

            res = await function.LeaveMeeting(req, context);

            res.StatusCode.Is(CommonResult.ValidateError);
            res.Headers.Is(CommonResult.ResponseHeader);
            res.Body.Is(string.Empty);

            _meetingMock.Verify(x => x.LeaveMeeting("meetingId", "attendeeId"), Times.Once);
            _dynamoDBMock.Verify(x => x.DeleteAttendeeInfo("attendeeId"), Times.Once);


            _meetingMock.Setup(x => x.LeaveMeeting("meetingId", "attendeeId"))
           .ThrowsAsync(new EnvironmentVariableException("bad environment variable"));

            reqBody.MeetingId = "meetingId";
            req.Body = JsonSerializer.Serialize(reqBody);

            res = await function.LeaveMeeting(req, context);
            res.StatusCode.Is(CommonResult.InternalServerError);
            res.Headers.Is(CommonResult.ResponseHeader);
            res.Body.Is(string.Empty);

            _meetingMock.Verify(x => x.LeaveMeeting("meetingId", "attendeeId"), Times.Exactly(2));
            _dynamoDBMock.Verify(x => x.DeleteAttendeeInfo("attendeeId"), Times.Once);

            _meetingMock.Setup(x => x.LeaveMeeting("meetingId", "attendeeId"))
           .ThrowsAsync(new Exception("exception"));

            res = await function.LeaveMeeting(req, context);
            res.StatusCode.Is(CommonResult.InternalServerError);
            res.Headers.Is(CommonResult.ResponseHeader);
            res.Body.Is(string.Empty);

            _meetingMock.Verify(x => x.LeaveMeeting("meetingId", "attendeeId"), Times.Exactly(3));
            _dynamoDBMock.Verify(x => x.DeleteAttendeeInfo("attendeeId"), Times.Once);
        }
    }

    public class LeaveMeetingRequestT
    {
        public string MeetingId { get; set; } = "meetingId";
        public string AttendeeId { get; set; } = "attendeeId";
    }
}
