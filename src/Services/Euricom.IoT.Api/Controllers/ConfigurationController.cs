using AutoMapper;
using Euricom.IoT.Api.Dtos;
using Euricom.IoT.Api.Managers;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Utilities;
using Euricom.IoT.Models;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using Restup.WebServer.Attributes;
using System;

namespace Euricom.IoT.Api.Controllers
{
    [Authorize]
    [RestController(InstanceCreationType.Singleton)]
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
                var settingsDto = Mapper.Map<Settings>(settings);
                return ResponseUtilities.GetResponseOk(settingsDto);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw new Exception(ex.Message);
            }
        }

        [UriFormat("/settings")]
        public IPutResponse SaveConfig([FromContent] SettingsDto settingsDto)
        {
            try
            {
                var settings = Mapper.Map<Settings>(settingsDto);
                _configurationManager.SaveConfigSettings(settings);
                return new PutResponse(PutResponse.ResponseStatus.OK);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                throw new Exception(ex.Message);
            }
        }

    }
}
