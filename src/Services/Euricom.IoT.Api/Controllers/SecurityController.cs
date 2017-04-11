using AutoMapper;
using Euricom.IoT.Api.Dtos;
using Euricom.IoT.Api.Managers;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Utilities;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using System;

namespace Euricom.IoT.Api.Controllers
{
    [RestController(InstanceCreationType.Singleton)]
    public class SecurityController
    {
        //private readonly ILogManager _logManager;

        public SecurityController()
        {
            // _logManager = new LogManager();
        }

        //[UriFormat("/security")]
        //public IPostResponse RequestCommandToken([FromContent] RequestForAccessToken request)
        //{
        //    try
        //    {
        //        var logFiles = _logManager.QueryLogFiles();
        //        return ResponseUtilities.GetResponseOk(logFiles);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
        //        return ResponseUtilities.GetResponseFail($"Could not get log: exception: {ex.Message}");
        //    }
        //}
    }
}
