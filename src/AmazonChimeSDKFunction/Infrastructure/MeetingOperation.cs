using Amazon.ChimeSDKMeetings;
using Amazon.ChimeSDKMeetings.Model;
using ChimeApp.Domain;

namespace ChimeApp.Infrastructure
{
    internal class MeetingOperation : IMeetingOperation
    {
        private readonly IAmazonChimeSDKMeetings _client;
        public MeetingOperation(IAmazonChimeSDKMeetings client)
        {
            _client = client;
        }
        public async Task<Meeting> CreateMeeting(Models.CreateMeetingRequest request)
        {
            Console.WriteLine(request.MaxAttendee);

            var response = await _client.CreateMeetingAsync(new CreateMeetingRequest
            {
                ClientRequestToken = Guid.NewGuid().ToString(),
                ExternalMeetingId = request.ExternalMeetingId,
                MediaRegion = Environment.GetEnvironmentVariable("MEDIA_REGION"),
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
