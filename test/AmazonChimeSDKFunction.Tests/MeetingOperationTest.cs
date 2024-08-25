using Xunit;
using Amazon.ChimeSDKMeetings;
using Amazon.ChimeSDKMeetings.Model;
using Moq;
using ChimeApp.Infrastructure;
using ChimeSDKServerApp.Domain.DomainHelper;

namespace AmazonChimeSDKFunction.Tests;

public class MeetingOperationTest
{
    private Mock<IAmazonChimeSDKMeetings> _mock;
    private Mock<DomainHelper> _mockDomainHelper;
    private MeetingOperation _meetingOperation;
    private CreateMeetingResponse _createMeetingResponse;
    private CreateAttendeeResponse _createAttendeeResponse;
    public MeetingOperationTest()
    {
        _mock = new Mock<IAmazonChimeSDKMeetings>();
        _mockDomainHelper = new Mock<DomainHelper>();
        _meetingOperation = new MeetingOperation(_mock.Object, _mockDomainHelper.Object);
        _createMeetingResponse = new CreateMeetingResponse();
        _createMeetingResponse.Meeting = new Meeting();
        _createAttendeeResponse = new CreateAttendeeResponse();
        _createAttendeeResponse.Attendee = new Attendee();
    }

    [Fact]
    [Trait("Category", "Infrastructure")]
    public async Task ミーティング作成()
    {
        var req = new ChimeApp.Models.CreateMeetingRequest();
        req.ExternalMeetingId = "externalMeetingId";

        _mockDomainHelper.Setup(x => x.GetUUId()).Returns("uuid");
        _mockDomainHelper.Setup(x => x.GetRegion()).Returns("region");
        _mock.Setup(x => x.CreateMeetingAsync(It.IsAny<CreateMeetingRequest>(), default))
            .Callback<CreateMeetingRequest, CancellationToken>((request, token) => {
                Assert.Equal("uuid", request.ClientRequestToken);
                Assert.Equal("externalMeetingId", request.ExternalMeetingId);
                Assert.Equal("region", request.MediaRegion);
                Assert.Equal(10, request.MeetingFeatures.Attendee.MaxCount);
            })
            .ReturnsAsync(_createMeetingResponse);

        var response = await _meetingOperation.CreateMeeting(req);

        Assert.NotNull(response);
        Assert.True(response.Equals(_createMeetingResponse.Meeting));

        _mock.Verify(x => x.CreateMeetingAsync(It.IsAny<CreateMeetingRequest>(), default), Times.Once);
    }

    [Fact]
    [Trait("Category", "Infrastructure")]
    public async Task ミーティング終了()
    {
        _mock.Setup(x => x.DeleteMeetingAsync(It.IsAny<DeleteMeetingRequest>(), default))
            .Callback<DeleteMeetingRequest, CancellationToken>((request, token) => { 
                Assert.Equal("meetingId", request.MeetingId);
            });

        await _meetingOperation.EndMeeting("meetingId");

        _mock.Verify(x => x.DeleteMeetingAsync(It.IsAny<DeleteMeetingRequest>(), default), Times.Once);
    }

    [Fact]
    [Trait("Category", "Infrastructure")]
    public async Task ミーティング参加()
    {
        _mock.Setup(x => x.CreateAttendeeAsync(It.IsAny<CreateAttendeeRequest>(), default))
            .Callback<CreateAttendeeRequest, CancellationToken>((request, token) =>
            {
                Assert.Equal("meetingId", request.MeetingId);
                Assert.Equal("externalAttendeeId", request.ExternalUserId);
            })
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
            .Callback<DeleteAttendeeRequest, CancellationToken>((request, token) =>
            {
                Assert.Equal("meetingId", request.MeetingId);
                Assert.Equal("attendeeId", request.AttendeeId);
            });

        await _meetingOperation.LeaveMeeting("meetingId", "attendeeId");

        _mock.Verify(x => x.DeleteAttendeeAsync(It.IsAny<DeleteAttendeeRequest>(), default), Times.Once);
    }
}
