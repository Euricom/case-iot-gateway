using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Models.Logging
{
    public class Log
    {
        public Log()
        {
            LogLines = new List<LogLine>();
        }

        public string FileName { get; set; }

        public IList<LogLine> LogLines { get; set; }
    }
}
