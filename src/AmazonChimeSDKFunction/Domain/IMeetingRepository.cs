using Amazon.ChimeSDKMeetings.Model;

namespace ChimeApp.Domain
{
    public interface IMeetingRepository
    {
        Task<Meeting> CreateMeeting(Models.CreateMeetingRequest request);
        Task<Attendee>? JoinMeeting(string meetingId, string externalAttendeeId);
        Task EndMeeting(string meetingId);
        Task LeaveMeeting(string meetingId, string attendeeId);
    }
}
