using AutoMapper;
using Euricom.IoT.Api.Dtos;
using Euricom.IoT.Api.Managers;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Utilities;
using Euricom.IoT.Common;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Euricom.IoT.Api.Controllers
{
    [RestController(InstanceCreationType.Singleton)]
    public class LogController
    {
        private readonly ILogManager _logManager;

        public LogController()
        {
            _logManager = new LogManager();
        }

        [UriFormat("/logs")]
        public IGetResponse QueryLogFiles()
        {
            try
            {
                var logFiles = _logManager.QueryLogFiles();
                return ResponseUtilities.GetResponseOk(logFiles);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                return ResponseUtilities.GetResponseFail($"Could not get log: exception: {ex.Message}");
            }
        }


        [UriFormat("/logs/{day}")]
        public IGetResponse GetAll(string day)
        {
            try
            {
                var log = _logManager.GetLog(day);
                var logDto = Mapper.Map<LogDto>(log);
                return ResponseUtilities.GetResponseOk(logDto);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                return ResponseUtilities.GetResponseFail($"Could not get log: exception: {ex.Message}");
            }
        }
    }
}
