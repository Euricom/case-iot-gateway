using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Logging;
using Euricom.IoT.Models.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Euricom.IoT.Api.Managers
{
    public class LogManager : ILogManager
    {
        public LogManager()
        {
        }

        public string[] QueryLogFiles()
        {
            return Logger.Instance.QueryLogFiles();
        }

        public string[] GetOpenZWaveLog()
        {
            var logLines = Logger.Instance.GetOpenZWaveLog();
            return logLines;
        }

        public Log GetLog(string date)
        {
            var logLines = Logger.Instance.GetLogLines(date);

            // Convert to json LogLine
            var jsonLogLines = DeserializeLogLines(logLines);

            // Get device name if found
            jsonLogLines = GetDeviceNames(jsonLogLines);

            // Order by date desc
            jsonLogLines = jsonLogLines.OrderByDescending(x => x.Timestamp).ToList();

            return new Log()
            {
                FileName = "log-" + date + ".txt",
                LogLines = jsonLogLines.ToList()
            };
        }

        private IEnumerable<LogLine> GetDeviceNames(IEnumerable<LogLine> jsonLogLines)
        {
            var copy = new List<LogLine>();
            foreach (var jLogLine in jsonLogLines)
            {
                if (jLogLine.Properties != null)
                {
                    if (!String.IsNullOrEmpty(jLogLine.Properties.DeviceId))
                    {
                        jLogLine.DeviceId = jLogLine.Properties.DeviceId;
                        jLogLine.DeviceName = new HardwareManager().GetDeviceName(jLogLine.Properties.DeviceId);
                    }
                }
                copy.Add(jLogLine);
            }
            return copy;
        }

        private IEnumerable<LogLine> DeserializeLogLines(string[] logLines)
        {
            foreach(var l in logLines)
            {
                yield return JsonConvert.DeserializeObject<LogLine>(l);
            }
        }
    }
}
