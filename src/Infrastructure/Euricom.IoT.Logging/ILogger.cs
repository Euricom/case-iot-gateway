using System;

namespace Euricom.IoT.Logging
{
    public interface ILogger
    {
        string[] GetLogLines(string date);
        string[] GetOpenZWaveLog();
        void LogDebugWithContext(Type type, string message);
        void LogDebugWithDeviceContext(string deviceId, string message);
        void LogError(Exception exception);
        void LogErrorWithContext(Type type, Exception exception);
        void LogErrorWithDeviceContext(string deviceId, Exception exception);
        void LogFatal(Exception exception);
        void LogInformationWithContext(Type type, string message);
        void LogInformationWithDeviceContext(string deviceId, string message);
        void LogVerboseWithContext(Type type, string message);
        void LogVerboseWithDeviceContext(string deviceId, string message);
        void LogWarningWithContext(Type type, string message);
        void LogWarningWithDeviceContext(string deviceId, string message);
        string[] QueryLogFiles();
    }
}