using System.Reflection;

namespace Logic.IServices
{
    public interface ILoggerManager
    {
        void LogDebug(MethodBase m, string message);
        void LogError(MethodBase m, string message);
        void Loginfo(MethodBase m, string message);
        void LogWarn(MethodBase m, string message);
    }
}
