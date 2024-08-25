using Amazon.ChimeSDKMeetings;
using Amazon.ChimeSDKMeetings.Model;
using ChimeApp.Domain;
using ChimeSDKServerApp.Domain.DomainHelper;

namespace ChimeApp.Infrastructure
{
    public class MeetingOperation : IMeetingOperation
    {
        private readonly IAmazonChimeSDKMeetings _client;
        private readonly DomainHelper _domainHelper;
        public MeetingOperation():this(new AmazonChimeSDKMeetingsClient(),
            new DomainHelper())
        {
        }

        public MeetingOperation(IAmazonChimeSDKMeetings client, DomainHelper helper)
        {
            _client = client;
            _domainHelper = helper;
        }
        public async Task<Meeting> CreateMeeting(Models.CreateMeetingRequest request)
        {
            var response = await _client.CreateMeetingAsync(new CreateMeetingRequest
            {
                ClientRequestToken = _domainHelper.GetUUId(),
                ExternalMeetingId = request.ExternalMeetingId,
                MediaRegion = _domainHelper.GetRegion(),
                MeetingFeatures = new MeetingFeaturesConfiguration(){
                    Attendee = new AttendeeFeatures(){
                        MaxCount = request.MaxAttendee == 0 ? 10 : request.MaxAttendee
                    }
                }
            });

            return response.Meeting;
        }

        public async Task EndMeeting(string meetingId)
        {
            var response = await _client.DeleteMeetingAsync(new DeleteMeetingRequest
            {
                MeetingId = meetingId
            });
        }

        public async Task<Attendee> JoinMeeting(string meetingId, string externalAttendeeId)
        {
            var response = await _client.CreateAttendeeAsync(new CreateAttendeeRequest
            {
                MeetingId = meetingId,
                ExternalUserId = externalAttendeeId
            });

            return response.Attendee;
        }

        public async Task LeaveMeeting(string meetingId, string attendeeId)
        {
            await _client.DeleteAttendeeAsync(new DeleteAttendeeRequest
            {
                MeetingId = meetingId,
                AttendeeId = attendeeId
            });
        }
    }
}
