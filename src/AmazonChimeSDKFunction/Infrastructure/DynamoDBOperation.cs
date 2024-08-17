using Amazon.ChimeSDKMeetings.Model;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using ChimeApp.Domain;
using Newtonsoft.Json;


namespace ChimeApp.Infrastructure
{
    internal class DynamoDBOperation: IDynamoDBOperation
    {
        private readonly IAmazonDynamoDB _client;
        private readonly string? _tableMeeting;
        private readonly string? _tableAttendee;
        public DynamoDBOperation(IAmazonDynamoDB client)
        {
            _client = client;
            _tableMeeting = Environment.GetEnvironmentVariable("TABLE_NAME_MEETING");
            _tableAttendee = Environment.GetEnvironmentVariable("TABLE_NAME_ATTENDEE");
        }

        public async Task<string> GetMeetingInfo(string externalMeetingId)
        {
            if (_tableMeeting == null)
            {
                throw new EnvironmentVariableException("A Environment variable of meeting table is null");
            }

            GetItemRequest req = new();
            req.Key = new Dictionary<string, AttributeValue>(){
                {"ExternalMeetingId", new AttributeValue{ S = externalMeetingId }}
            };
            req.TableName = _tableMeeting;

            var response = await _client.GetItemAsync(req);
            if (response == null || response.Item.Count == 0)
            {
                Console.WriteLine("Failed to get meeting info from dynamodb");
                return string.Empty;
            }

            return response.Item["MeetingInfo"].S;
        }

        public async Task<string> GetAttendeeInfo(string attendeeId)
        {
            if (_tableAttendee == null)
            {
                throw new EnvironmentVariableException("A Environment variable of attendee table is null");
            }

            GetItemRequest req = new();
            req.Key = new Dictionary<string, AttributeValue>(){
                {"AttendeeId", new AttributeValue{ S = attendeeId }}
            };
            req.TableName = _tableAttendee;

            var response = await _client.GetItemAsync(req);
            if (response == null || response.Item.Count == 0)
            {
                Console.WriteLine("Failed to get attendee info from dynamodb");
                return string.Empty;
            }

            return response.Item["ExternalAttendeeId"].S;
        }

        public async Task RegisterMeetingInfo(Meeting meeting)
        {
            if (_tableMeeting == null)
            {
                throw new EnvironmentVariableException("A Environment variable of meeting table is null");
            }

            var res = await _client.PutItemAsync(new PutItemRequest
            {
                TableName = _tableMeeting,
                Item = new Dictionary<string, AttributeValue>
                {
                    {"ExternalMeetingId", new AttributeValue{ S = meeting.ExternalMeetingId }},
                    {"MeetingId", new AttributeValue{ S = meeting.MeetingId }},
                    {"MeetingInfo", new AttributeValue{ S = JsonConvert.SerializeObject(meeting) }}
                }
            });
        }

        public async Task RegisterAttendeeInfo(Attendee attendee)
        {
            if (_tableAttendee == null)
            {
                throw new EnvironmentVariableException("A Environment variable of attendee table is null");
            }

            var res = await _client.PutItemAsync(new PutItemRequest
            {
                TableName = _tableAttendee,
                Item = new Dictionary<string, AttributeValue>
                {
                    {"AttendeeId", new AttributeValue{ S = attendee.AttendeeId }},
                    {"ExternalAttendeeId", new AttributeValue{ S = attendee.ExternalUserId }}
                }
            });
        }

        public async Task DeleteMeetingInfo(string externalMeetingId)
        {
            if (_tableMeeting == null)
            {
                throw new EnvironmentVariableException("A Environment variable of meeting table is null");
            }

            DeleteItemRequest req = new();
            req.TableName = _tableMeeting;
            req.Key["ExternalMeetingId"] = new AttributeValue { S = externalMeetingId };
            var res = await _client.DeleteItemAsync(req);
        }

        public async Task DeleteAttendeeInfo(string attendeeId)
        {
            if (_tableAttendee == null)
            {
                throw new EnvironmentVariableException("A Environment variable of attendee table is null");
            }

            DeleteItemRequest req = new();
            req.TableName = _tableAttendee;
            req.Key["AttendeeId"] = new AttributeValue { S = attendeeId };
            var res = await _client.DeleteItemAsync(req);
        }
    }
}
