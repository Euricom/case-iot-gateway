using AutoMapper;
using Euricom.IoT.Api.Dtos;
using Euricom.IoT.Api.Managers;
using Euricom.IoT.Api.Managers.Interfaces;
using Euricom.IoT.Api.Utilities;
using Euricom.IoT.Common;
using Euricom.IoT.DataLayer;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using System;

namespace Euricom.IoT.Api.Controllers
{
    [RestController(InstanceCreationType.Singleton)]
    public class ConfigurationController
    {
        private readonly IConfigurationManager _configurationManager;

        public ConfigurationController()
        {
            _configurationManager = new ConfigurationManager();
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
                return ResponseUtilities.GetResponseFail(ex.Message);
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
                return ResponseUtilities.PutResponseFail(ex.Message);
            }
        }

    }
}
