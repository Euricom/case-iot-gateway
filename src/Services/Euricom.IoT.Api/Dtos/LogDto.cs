using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Dtos
{
    public class LogDto
    {
        public LogDto()
        {
            LogLines = new List<LogLineDto>();
        }

        public string FileName { get; set; }

        public IList<LogLineDto> LogLines { get; set; }
    }
}
