using Amazon.ChimeSDKMeetings.Model;

namespace ChimeApp.Domain
{
    internal interface IDynamoDBRepository
    {
        Task<string> GetMeetingInfo(string externalMeetingId);
        Task<string> GetAttendeeInfo(string attendeeId);
        Task RegisterMeetingInfo(Meeting meeting);
        Task RegisterAttendeeInfo(Attendee attendee);
        Task DeleteMeetingInfo(string externalMeetingId);
        Task DeleteAttendeeInfo(string attendeeId);
    }
}
