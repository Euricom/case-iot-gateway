using Euricom.IoT.Models.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Logging
{
    public class Logger : ILogger
    {
        private static string _path;
        private static volatile Logger _instance;
        private static object _syncRoot = new Object(); //for thread safe singleton

        private static object _syncRootContextLogger = new Object(); //For locking the logging context (if two threads try to log from two different managers/device id, the context must be correct)

        private Serilog.Core.Logger _logger;

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
        }

        public static Logger Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                            _instance = new Logger();
                    }
                }

                return _instance;
            }
        }

        public void Init()
        {
            Debug.WriteLine("Logger Init()");

            Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));

            string logDirectory = Path.Combine(_path, "logs");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            // string template = "{Timestamp} [{Level}] {Message}{NewLine}{Exception}";
            string pathFormat = logDirectory + "\\" + "log-{Date}.txt";
            var jsonFormatter = new JsonFormatter(null, false, null);

            var logger = new LoggerConfiguration()
            .WriteTo.RollingFile(jsonFormatter, pathFormat, _logEventLevel, null, _historyLog, null, false, false, null) //Third parameter from right is buffered
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
            string logDirectory = System.IO.Path.Combine(_path, "logs");
            string logFile = System.IO.Path.Combine(logDirectory, "log-" + date + ".txt");
            if (System.IO.File.Exists(logFile))
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

        public void LogVerboseWithDeviceContext(string deviceId, string message)
        {
            lock (_syncRootContextLogger)
            {
                var contextLogger = _logger.ForContext("DeviceId", deviceId);
                contextLogger.Verbose(message);
            }
        }

        public void LogVerboseWithContext(Type type, string message)
        {
            lock (_syncRootContextLogger)
            {
                var contextLogger = _logger.ForContext(type);
                contextLogger.Verbose(message);
            }
        }

        public void LogDebugWithDeviceContext(string deviceId, string message)
        {
            lock (_syncRootContextLogger)
            {
                var contextLogger = _logger.ForContext("DeviceId", deviceId);
                contextLogger.Debug(message);
            }
        }

        public void LogDebugWithContext(Type type, string message)
        {
            lock (_syncRootContextLogger)
            {
                var contextLogger = _logger.ForContext(type);
                contextLogger.Debug(message);
            }
        }

        public void LogInformationWithDeviceContext(string deviceId, string message)
        {
            lock (_syncRootContextLogger)
            {
                var contextLogger = _logger.ForContext("DeviceId", deviceId);
                contextLogger.Information(message);
            }
        }

        public void LogWarningWithDeviceContext(string deviceId, string message)
        {
            lock (_syncRootContextLogger)
            {
                var contextLogger = _logger.ForContext("DeviceId", deviceId);
                contextLogger.Warning(message);
            }
        }

        public void LogErrorWithDeviceContext(string deviceId, Exception exception)
        {
            lock (_syncRootContextLogger)
            {
                var contextLogger = _logger.ForContext("DeviceId", deviceId);
                contextLogger.Error(exception, "");
            }
        }

        public void LogInformationWithContext(Type type, string message)
        {
            lock (_syncRootContextLogger)
            {
                var contextLogger = _logger.ForContext(type);
                contextLogger.Information(message);
            }
        }

        public void LogWarningWithContext(Type type, string message)
        {
            lock (_syncRootContextLogger)
            {
                var contextLogger = _logger.ForContext(type);
                contextLogger.Warning(message);
            }
        }

        public void LogErrorWithContext(Type type, Exception exception)
        {
            lock (_syncRootContextLogger)
            {
                var contextLogger = _logger.ForContext(type);
                contextLogger.Error(exception, "");
            }
        }

        public void LogError(Exception exception)
        {
            _logger.Error(exception, "");
        }

        public void LogFatal(Exception exception)
        {
            _logger.Fatal(exception, "");
        }
    }
}
