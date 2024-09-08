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
    public class CreateMeetingFunctionTest
    {
        private readonly Mock<IMeetingRepository> _meetingMock;
        private readonly Mock<IDynamoDBRepository> _dynamoDBMock;
        public CreateMeetingFunctionTest()
        {
            _meetingMock = new Mock<IMeetingRepository>();
            _dynamoDBMock = new Mock<IDynamoDBRepository>();   
        }

        [Fact]
        [Trait("Category", "LambdaFunction")]
        public async Task ミーティング開始ロジックのテスト()
        {
            var meeting = new Amazon.ChimeSDKMeetings.Model.Meeting()
            {
                MeetingId = "meetingId"
            };
            var attendee = new Amazon.ChimeSDKMeetings.Model.Attendee();
            _meetingMock.Setup(x => x.CreateMeeting(It.IsAny<CreateMeetingRequest>()))
            .ReturnsAsync(meeting);
            _meetingMock.Setup(x => x.JoinMeeting(meeting.MeetingId, "externalAttendeeId"))
            .ReturnsAsync(attendee);

            _dynamoDBMock.Setup(x => x.RegisterMeetingInfo(meeting))
            .Callback<Amazon.ChimeSDKMeetings.Model.Meeting>((request) =>
            {
                request.Is(meeting);
            });
            _dynamoDBMock.Setup(x => x.RegisterAttendeeInfo(attendee))
            .Callback<Amazon.ChimeSDKMeetings.Model.Attendee>((request) =>
            {
                request.Is(attendee);
            });


            var function = new CreateMeetingFunction(_meetingMock.Object, _dynamoDBMock.Object);
            var context = new TestLambdaContext();
            var req = new APIGatewayProxyRequest();
            var reqBody = new CreateMeetingRequestT();
            req.Body = System.Text.Json.JsonSerializer.Serialize(reqBody);

            var meetingStr = System.Text.Json.JsonSerializer.Serialize(meeting);
            var attendeeStr = System.Text.Json.JsonSerializer.Serialize(attendee);
            var expected = new CreateMeetingResponse()
            {
                MeetingInfo = meetingStr,
                AttendeeInfo = attendeeStr
            };

            var res = await function.CreateMeeting(req, context);

            res.StatusCode.Is(CommonResult.OK);
            res.Headers.Is(CommonResult.ResponseHeader);
            res.Body.Is(JsonConvert.SerializeObject(expected));

            _meetingMock.Verify(x => x.CreateMeeting(It.IsAny<CreateMeetingRequest>()), Times.Once);
            _meetingMock.Verify(x => x.JoinMeeting(meeting.MeetingId, "externalAttendeeId"), Times.Once);
            _dynamoDBMock.Verify(x => x.RegisterMeetingInfo(meeting), Times.Once);
            _dynamoDBMock.Verify(x => x.RegisterAttendeeInfo(attendee), Times.Once);

            reqBody.ExternalMeetingId = string.Empty;
            req.Body = System.Text.Json.JsonSerializer.Serialize(reqBody);

            res = await function.CreateMeeting(req, context);

            res.StatusCode.Is(CommonResult.ValidateError);
            res.Headers.Is(CommonResult.ResponseHeader);
            res.Body.Is(string.Empty);

            _meetingMock.Verify(x => x.CreateMeeting(It.IsAny<CreateMeetingRequest>()), Times.Once);
            _meetingMock.Verify(x => x.JoinMeeting(meeting.MeetingId, "externalAttendeeId"), Times.Once);
            _dynamoDBMock.Verify(x => x.RegisterMeetingInfo(meeting), Times.Once);
            _dynamoDBMock.Verify(x => x.RegisterAttendeeInfo(attendee), Times.Once);


            _meetingMock.Setup(x => x.CreateMeeting(It.IsAny<CreateMeetingRequest>()))
           .ThrowsAsync(new EnvironmentVariableException("bad environment variable"));

            reqBody.ExternalMeetingId = "externalMeetingId";
            req.Body = System.Text.Json.JsonSerializer.Serialize(reqBody);

            res = await function.CreateMeeting(req, context);
            res.StatusCode.Is(CommonResult.InternalServerError);
            res.Headers.Is(CommonResult.ResponseHeader);
            res.Body.Is(string.Empty);

            _meetingMock.Verify(x => x.CreateMeeting(It.IsAny<CreateMeetingRequest>()), Times.Exactly(2));
            _meetingMock.Verify(x => x.JoinMeeting(meeting.MeetingId, "externalAttendeeId"), Times.Once);
            _dynamoDBMock.Verify(x => x.RegisterMeetingInfo(meeting), Times.Once);
            _dynamoDBMock.Verify(x => x.RegisterAttendeeInfo(attendee), Times.Once);

            _meetingMock.Setup(x => x.CreateMeeting(It.IsAny<CreateMeetingRequest>()))
           .ThrowsAsync(new Exception("exception"));

            res = await function.CreateMeeting(req, context);
            res.StatusCode.Is(CommonResult.InternalServerError);
            res.Headers.Is(CommonResult.ResponseHeader);
            res.Body.Is(string.Empty);

            _meetingMock.Verify(x => x.CreateMeeting(It.IsAny<CreateMeetingRequest>()), Times.Exactly(3));
            _meetingMock.Verify(x => x.JoinMeeting(meeting.MeetingId, "externalAttendeeId"), Times.Once);
            _dynamoDBMock.Verify(x => x.RegisterMeetingInfo(meeting), Times.Once);
            _dynamoDBMock.Verify(x => x.RegisterAttendeeInfo(attendee), Times.Once);
        }
    }

    public class CreateMeetingRequestT
    {
        public string ExternalMeetingId { get; set; } = "externalMeetingId";
        public string ExternalAttendeeId { get; set; } = "externalAttendeeId";
        public int MaxAttendee { get; set; } = 10;
    }
}
