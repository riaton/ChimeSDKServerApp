namespace ChimeSDKServerApp.Domain.DomainHelper
{
    public class DomainHelper
    {
        public virtual string GetUUId()
        {
            return Guid.NewGuid().ToString();
        }

        public virtual string GetRegion() {             
            return Environment.GetEnvironmentVariable("MEDIA_REGION") ?? string.Empty;
        }
    }
}
