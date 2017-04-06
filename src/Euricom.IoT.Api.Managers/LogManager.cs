using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.AzureDeviceManager;
using Euricom.IoT.Common;
using Euricom.IoT.Common.Notifications;
using Euricom.IoT.Common.Utilities;
using Euricom.IoT.DataLayer;
using Euricom.IoT.LazyBone;
using Euricom.IoT.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public Log GetLog(string date)
        {
            var logLines = Logger.Instance.GetLogLines(date);

            // Convert to json LogLine
            var jsonLogLines = DeserializeLogLines(logLines);

            return new Log()
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
