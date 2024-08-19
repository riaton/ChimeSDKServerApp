using Xunit;
using Amazon.ChimeSDKMeetings;
using Amazon.ChimeSDKMeetings.Model;
using Moq;
using ChimeApp.Infrastructure;

namespace AmazonChimeSDKFunction.Tests;

public class MeetingOperationTest
{
    private Mock<IAmazonChimeSDKMeetings> _mock;
    private MeetingOperation _meetingOperation;
    private CreateMeetingResponse _createMeetingResponse;
    private CreateAttendeeResponse _createAttendeeResponse;
    public MeetingOperationTest()
    {
        _mock = new Mock<IAmazonChimeSDKMeetings>();
        _meetingOperation = new MeetingOperation(_mock.Object);
        _createMeetingResponse = new CreateMeetingResponse();
        _createMeetingResponse.Meeting = new Meeting();
        _createAttendeeResponse = new CreateAttendeeResponse();
        _createAttendeeResponse.Attendee = new Attendee();
    }

    [Fact]
    [Trait("Category", "Infrastructure")]
    public async Task ミーティング作成()
    { 
        _mock.Setup(x => x.CreateMeetingAsync(It.IsAny<CreateMeetingRequest>(), default))
            .ReturnsAsync(_createMeetingResponse);

        var response = await _meetingOperation.CreateMeeting(new ChimeApp.Models.CreateMeetingRequest());

        Assert.NotNull(response);
        Assert.True(response.Equals(_createMeetingResponse.Meeting));

        _mock.Verify(x => x.CreateMeetingAsync(It.IsAny<CreateMeetingRequest>(), default), Times.Once);
    }

    [Fact]
    [Trait("Category", "Infrastructure")]
    public async Task ミーティング終了()
    {
        _mock.Setup(x => x.DeleteMeetingAsync(It.IsAny<DeleteMeetingRequest>(), default))
            .ReturnsAsync(new DeleteMeetingResponse());

        await _meetingOperation.EndMeeting("meetingId");

        _mock.Verify(x => x.DeleteMeetingAsync(It.IsAny<DeleteMeetingRequest>(), default), Times.Once);
    }

    [Fact]
    [Trait("Category", "Infrastructure")]
    public async Task ミーティング参加()
    {
        _mock.Setup(x => x.CreateAttendeeAsync(It.IsAny<CreateAttendeeRequest>(), default))
            .ReturnsAsync(_createAttendeeResponse);

        var response = await _meetingOperation.JoinMeeting("meetingId", "externalAttendeeId");

        Assert.NotNull(response);
        Assert.True(response.Equals(_createAttendeeResponse.Attendee));

        _mock.Verify(x => x.CreateAttendeeAsync(It.IsAny<CreateAttendeeRequest>(), default), Times.Once);
    }

    [Fact]
    [Trait("Category", "Infrastructure")]
    public async Task ミーティング退室()
    {
        _mock.Setup(x => x.DeleteAttendeeAsync(It.IsAny<DeleteAttendeeRequest>(), default))
            .ReturnsAsync(new DeleteAttendeeResponse());

        await _meetingOperation.LeaveMeeting("meetingId", "attendeeId");

        _mock.Verify(x => x.DeleteAttendeeAsync(It.IsAny<DeleteAttendeeRequest>(), default), Times.Once);
    }
}
