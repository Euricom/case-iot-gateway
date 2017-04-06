using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Euricom.IoT.Logging
{
    public class Logger : ILogger
    {
        private static volatile Logger _instance;
        private static object _syncRoot = new Object(); //for thread safe singleton

        private static object _syncRootContextLogger = new Object(); //For locking the logging context (if two threads try to log from two different managers/device id, the context must be correct)

        private Serilog.Core.Logger _logger;

        private Logger()
        {
            Init().Wait();
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

        private async Task Init()
        {
            try
            {
                Debug.WriteLine("Logger Init()");

                Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));

                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                string logDirectory = System.IO.Path.Combine(localFolder.Path, "logs");
                if (!System.IO.Directory.Exists(logDirectory))
                {
                    await localFolder.CreateFolderAsync("logs");
                }

                // string template = "{Timestamp} [{Level}] {Message}{NewLine}{Exception}";
                string pathFormat = logDirectory + "\\" + "log-{Date}.txt";
                var jsonFormatter = new JsonFormatter(null, false, null);

                var logger = new LoggerConfiguration()
                .WriteTo.RollingFile(jsonFormatter, pathFormat, LogEventLevel.Information, null, 31, null, false, false, null) //Third parameter from right is buffered
                .CreateLogger();

                _logger = logger;

                _logger.Information("Logging started");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string[] QueryLogFiles()
        {
            var logFiles = new List<string>();
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            string logDirectory = System.IO.Path.Combine(localFolder.Path, "logs");
            var files = System.IO.Directory.GetFiles(logDirectory);
            return files;
        }

        public string[] GetLogLines(string date)
        {
            var logLines = new List<string>();
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            string logDirectory = System.IO.Path.Combine(localFolder.Path, "logs");
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

        public void LogInformationWithDeviceContext(string deviceId, string message)
        {
            lock(_syncRootContextLogger)
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
                var contextLogger =_logger.ForContext("DeviceId", deviceId);
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
                var contextLogger =_logger.ForContext(type);
                contextLogger.Error(exception, "");
            }
        }

        public void LogError(Exception exception)
        {
            _logger.Error(exception, "");
        }
    }
}
