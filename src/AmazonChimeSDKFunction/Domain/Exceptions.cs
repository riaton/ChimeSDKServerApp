namespace ChimeApp.Domain
{
    internal class EnvironmentVariableException : Exception
    {
        public EnvironmentVariableException(string message) : base(message)
        {
        }
    }
}
