using Xunit;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;
using ChimeApp.LambdaFunctions;
using ChimeApp.Domain;
using Moq;
using ChimeApp.Models;
using Newtonsoft.Json;

namespace ChimeSDKServerApp.Tests.LambdaFunctionTests
{
    public class JoinMeetingFunctionTest
    {
        private readonly Mock<IMeetingRepository> _meetingMock;
        private readonly Mock<IDynamoDBRepository> _dynamoDBMock;
        public JoinMeetingFunctionTest()
        {
            _meetingMock = new Mock<IMeetingRepository>();
            _dynamoDBMock = new Mock<IDynamoDBRepository>();   
        }

        [Fact]
        [Trait("Category", "LambdaFunction")]
        public async Task ミーティング参加ロジックのテスト()
        {
            var attendee = new Amazon.ChimeSDKMeetings.Model.Attendee();
            _meetingMock.Setup(x => x.JoinMeeting("meetingId", "externalAttendeeId"))
            .ReturnsAsync(attendee);

            _dynamoDBMock.Setup(x => x.RegisterAttendeeInfo(attendee))
            .Callback<Amazon.ChimeSDKMeetings.Model.Attendee>((request) =>
            {
                request.Is(attendee);
            });

            var function = new JoinMeetingFunction(_meetingMock.Object, _dynamoDBMock.Object);
            var context = new TestLambdaContext();
            var req = new APIGatewayProxyRequest();
            var reqBody = new JoinMeetingRequestT();
            req.Body = System.Text.Json.JsonSerializer.Serialize(reqBody);

            var attendeeStr = System.Text.Json.JsonSerializer.Serialize(attendee);
            var expected = new JoinMeetingResponse()
            {
                AttendeeInfo = attendeeStr
            };

            var res = await function.JoinMeeting(req, context);

            res.StatusCode.Is(CommonResult.OK);
            res.Headers.Is(CommonResult.ResponseHeader);
            res.Body.Is(JsonConvert.SerializeObject(expected));

            _meetingMock.Verify(x => x.JoinMeeting("meetingId", "externalAttendeeId"), Times.Once);
            _dynamoDBMock.Verify(x => x.RegisterAttendeeInfo(attendee), Times.Once);

            reqBody.MeetingId = string.Empty;
            req.Body = System.Text.Json.JsonSerializer.Serialize(reqBody);

            res = await function.JoinMeeting(req, context);

            res.StatusCode.Is(CommonResult.ValidateError);
            res.Headers.Is(CommonResult.ResponseHeader);
            res.Body.Is(string.Empty);

            _meetingMock.Verify(x => x.JoinMeeting("meetingId", "externalAttendeeId"), Times.Once);
            _dynamoDBMock.Verify(x => x.RegisterAttendeeInfo(attendee), Times.Once);


            _meetingMock.Setup(x => x.JoinMeeting("meetingId", "externalAttendeeId"))
           .ThrowsAsync(new EnvironmentVariableException("bad environment variable"));

            reqBody.MeetingId = "meetingId";
            req.Body = System.Text.Json.JsonSerializer.Serialize(reqBody);

            res = await function.JoinMeeting(req, context);
            res.StatusCode.Is(CommonResult.InternalServerError);
            res.Headers.Is(CommonResult.ResponseHeader);
            res.Body.Is(string.Empty);

            _meetingMock.Verify(x => x.JoinMeeting("meetingId", "externalAttendeeId"), Times.Exactly(2));
            _dynamoDBMock.Verify(x => x.RegisterAttendeeInfo(attendee), Times.Once);

            _meetingMock.Setup(x => x.JoinMeeting("meetingId", "externalAttendeeId"))
           .ThrowsAsync(new Exception("exception"));

            res = await function.JoinMeeting(req, context);
            res.StatusCode.Is(CommonResult.InternalServerError);
            res.Headers.Is(CommonResult.ResponseHeader);
            res.Body.Is(string.Empty);

            _meetingMock.Verify(x => x.JoinMeeting("meetingId", "externalAttendeeId"), Times.Exactly(3));
            _dynamoDBMock.Verify(x => x.RegisterAttendeeInfo(attendee), Times.Once);
        }
    }

    public class JoinMeetingRequestT
    {
        public string MeetingId { get; set; } = "meetingId";
        public string ExternalAttendeeId { get; set; } = "externalAttendeeId";
    }
}
