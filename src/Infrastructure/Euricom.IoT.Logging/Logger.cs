using Euricom.IoT.Models.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Euricom.IoT.Logging
{
    public class Logger : ILogger
    {
        private static string _path;

        private static Lazy<Logger> _lazy =
            new Lazy<Logger>(() => new Logger());

        private Serilog.ILogger _logger;

        private static int _historyLog = 31;
        private static LogEventLevel _logEventLevel = LogEventLevel.Information;
        
        private Logger()
        {
            Init();
        }

        public static void Configure(int historyLog, LogLevel logLevel, string path)
        {
            _path = path;
            _historyLog = historyLog;
            _logEventLevel = (LogEventLevel)Enum.Parse(typeof(LogEventLevel), logLevel.ToString());

            _lazy = new Lazy<Logger>(() => new Logger());
        }

        public static ILogger Instance => _lazy.Value;

        private void Init()
        {
            System.Diagnostics.Debug.WriteLine("Logger Init()");

            Serilog.Debugging.SelfLog.Enable(msg => System.Diagnostics.Debug.WriteLine(msg));

            string logDirectory = Path.Combine(_path, "logs");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            string pathFormat = logDirectory + "\\" + "log-{Date}.txt";

            var jsonFormatter = new JsonFormatter();

            var logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.RollingFile(jsonFormatter, pathFormat, _logEventLevel, null, _historyLog, null, false, false, null)
                .CreateLogger();

            _logger = logger;

            _logger.Information($"Logging started: history log will be saved for {_historyLog} days");
        }

        public string[] QueryLogFiles()
        {
            string logDirectory = Path.Combine(_path, "logs");
            var files = Directory.GetFiles(logDirectory);
            return files;
        }

        public string[] GetLogLines(string date)
        {
            var logLines = new List<string>();
            string logDirectory = Path.Combine(_path, "logs");
            string logFile = Path.Combine(logDirectory, "log-" + date + ".txt");
            if (File.Exists(logFile))
            {
                using (var fileStream = File.Open(logFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        logLines.Add(line);
                    }
                }
                return logLines.ToArray();
            }
            throw new Exception($"Could not get logFile {logFile} from logDirectory {logDirectory}");
        }

        public string[] GetOpenZWaveLog()
        {
            var logLines = new List<string>();
            string logDirectory = Path.Combine(_path);
            string logFile = Path.Combine(logDirectory, "OZW_Log.txt");
            if (File.Exists(logFile))
            {
                using (var fileStream = File.Open(logFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        logLines.Add(line);
                    }
                }
                return logLines.ToArray();
            }
            throw new Exception($"Could not get logFile {logFile} from logDirectory {logDirectory}");
        }

        public bool IsEnabled(LogLevel level)
        {
            return _logger.IsEnabled((LogEventLevel)Enum.Parse(typeof(LogEventLevel), level.ToString()));
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Verbose(string message)
        {
            _logger.Verbose(message);
        }

        public void Information(string message)
        {
            _logger.Information(message);
        }

        public void Warning(string message)
        {
            _logger.Warning(message);
        }

        public void Error(Exception exception, string message = null)
        {
            _logger.Error(exception, message);
        }

        public void Fatal(Exception exception, string message = null)
        {
            _logger.Fatal(exception, message);
        }
    }
}
