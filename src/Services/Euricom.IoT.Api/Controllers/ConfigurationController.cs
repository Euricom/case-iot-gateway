using AutoMapper;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Utilities;
using Euricom.IoT.Models;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using Restup.WebServer.Attributes;
using System;
using Euricom.IoT.Api.Models;

namespace Euricom.IoT.Api.Controllers
{
    [Authorize("Administrator")]
    [RestController]
    public class ConfigurationController
    {
        private readonly IConfigurationManager _configurationManager;

        public ConfigurationController(IConfigurationManager configurationManager)
        {
            _configurationManager = configurationManager;
        }

        [UriFormat("/settings")]
        public IGetResponse GetConfig()
        {
            try
            {
                var settings = _configurationManager.GetConfigSettings();
                return ResponseUtilities.GetResponseOk(settings);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
        }

        [UriFormat("/settings")]
        public IPutResponse SaveConfig([FromContent] SettingsDto settingsDto)
        {
            try
            {
                _configurationManager.SaveConfigSettings(settingsDto);
                return new PutResponse(PutResponse.ResponseStatus.OK);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
        }
    }
}
