using Logic.IServices;
using NLog;
using System.Reflection;

namespace Logic.Services
{
    public class LoggerManager : ILoggerManager
    {
        private static NLog.ILogger logger = LogManager.GetCurrentClassLogger();

        public void LogDebug(MethodBase m, string message)
        {
            (string className, string methodName) = GetClassAndMethodName(m!);
            var msg = $"{className} ::> {methodName} :: {message}";
            logger.Debug(msg);
        }
        public void LogError(MethodBase m, string message)
        {
            (string className, string methodName) = GetClassAndMethodName(m!);
            var msg = $"{className} ::> {methodName} :: {message}";
            logger.Error(msg);
        }
        public void Loginfo(MethodBase m, string message)
        {
            (string className, string methodName) = GetClassAndMethodName(m!);
            var msg = $"{className} ::> {methodName} :: {message}";
            logger.Info(msg);
        }
        public void LogWarn(MethodBase m, string message)
        {
            (string className, string methodName) = GetClassAndMethodName(m!);
            var msg = $"{className} ::> {methodName} :: {message}";
            logger.Warn(msg);
        }

        public (string, string) GetClassAndMethodName(MethodBase m)
        {
            if (m == null)
                throw new ArgumentNullException(nameof(m));
            var className = m?.DeclaringType?.FullName;
            if (className.Contains("+<"))
            {
                className = className.Split("+<")[0];
            }
            var methodName = m?.DeclaringType?.Name ?? "UnknownMethod";
            if (methodName.Contains("<") && methodName.Contains(">"))
            {
                var methodNameParts = methodName.Split(new[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries);
                if (methodNameParts.Length >= 2)
                {
                    methodName = methodNameParts[0];
                }
            }
            return (className, methodName);
        }
    }
}
