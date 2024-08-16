using Amazon.ChimeSDKMeetings.Model;

namespace ChimeApp.Domain
{
    internal interface IMeetingOperation
    {
        Task<Meeting> CreateMeeting(Models.CreateMeetingRequest request);
        Task<Attendee>? JoinMeeting(string meetingId, string externalAttendeeId);
        Task EndMeeting(string meetingId);
        Task LeaveMeeting(string meetingId, string attendeeId);
    }
}
