using AutoMapper;
using Euricom.IoT.Api.Dtos;
using Euricom.IoT.Api.Managers;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Utilities;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using Restup.WebServer.Attributes;
using System;

namespace Euricom.IoT.Api.Controllers
{
    [Authorize]
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
                throw new Exception($"Could not get log: exception: {ex.Message}");
            }
        }

        [UriFormat("/logs_openzwave")]
        public IGetResponse GetOpenZWaveLog()
        {
            try
            {
                var logFiles = _logManager.GetOpenZWaveLog();
                return ResponseUtilities.GetResponseOk(logFiles);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw new Exception($"Could not get log: exception: {ex.Message}");
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
                throw new Exception($"Could not get log: exception: {ex.Message}");
            }
        }
    }
}
