using System;

namespace Euricom.IoT.Logging
{
    public interface ILogger
    {
        string[] GetLogLines(string date);

        void LogError(Exception exception);

        void LogInformationWithDeviceContext(string deviceId, string message);
        void LogWarningWithDeviceContext(string deviceId, string message);
        void LogErrorWithDeviceContext(string deviceId, Exception exception);

        void LogErrorWithContext(Type type, Exception exception);
        void LogInformationWithContext(Type type, string message);
        void LogWarningWithContext(Type type, string message);
    }
}