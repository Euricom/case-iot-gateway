using System.Collections.Generic;

namespace Euricom.IoT.Api.Models
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
