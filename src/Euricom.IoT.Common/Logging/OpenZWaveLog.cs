using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Common.Logging
{
    public class OpenZWaveLog
    {
        public List<string> LogLines { get; set; }

        public OpenZWaveLog()
        {
            LogLines = new List<String>();
        }

    }
}
