namespace Services.Logging
{
    public interface ILogger
    {
        void Log(string WhoSends, string Message, params object[] args);
        void LogInfo(string WhoSends, string Message, params object[] args);
        void LogWarning(string WhoSends, string Message, params object[] args);
        void LogError(string WhoSends, string Message, params object[] args);
        void LogFatal(string WhoSends, string Message, params object[] args);
    }
}
