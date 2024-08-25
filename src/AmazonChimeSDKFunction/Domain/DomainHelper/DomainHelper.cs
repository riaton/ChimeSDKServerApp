namespace ChimeSDKServerApp.Domain.DomainHelper
{
    public class DomainHelper
    {
        public virtual string GetUUId()
        {
            return Guid.NewGuid().ToString();
        }

        public virtual string? GetRegion() {             
            return Environment.GetEnvironmentVariable("MEDIA_REGION");
        }

        public virtual string? GetMeetingTableName() {
            return Environment.GetEnvironmentVariable("TABLE_NAME_MEETING");
        }

        public virtual string? GetAttendeeTableName() {
            return Environment.GetEnvironmentVariable("TABLE_NAME_ATTENDEE");
        }
    }
}
