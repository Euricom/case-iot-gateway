using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Models.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Euricom.IoT.Logging;

namespace Euricom.IoT.Api.Managers
{
    public class LogManager : ILogManager
    {
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
            
            // Order by date desc
            jsonLogLines = jsonLogLines.OrderByDescending(x => x.Timestamp).ToList();

            return new Log
            {
                FileName = "log-" + date + ".txt",
                LogLines = jsonLogLines.ToList()
            };
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
