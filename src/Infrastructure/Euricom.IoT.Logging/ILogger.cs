using System;
using Euricom.IoT.Models.Logging;

namespace Euricom.IoT.Logging
{
    public interface ILogger
    {
        string[] GetLogLines(string date);
        string[] GetOpenZWaveLog();
        string[] QueryLogFiles();

        bool IsEnabled(LogLevel level);
        void Debug(string message);
        void Verbose(string message);
        void Information(string message);
        void Warning(string message);
        void Error(Exception exception, string message = null);
        void Fatal(Exception exception, string message = null);
    }
}