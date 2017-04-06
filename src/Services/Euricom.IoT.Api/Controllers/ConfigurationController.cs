using Euricom.IoT.Api.Utilities;
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
        public ConfigurationController()
        {
        }

        [UriFormat("/settings")]
        public IGetResponse GetConfig()
        {
            try
            {
                var settings = Database.Instance.GetConfigSettings();
                return ResponseUtilities.GetResponseOk(settings);
            }
            catch (Exception ex)
            {
                Logging.Logger.Instance.LogErrorWithContext(this.GetType(), ex);
                return ResponseUtilities.GetResponseFail(ex.Message);
            }
        }

        [UriFormat("/settings")]
        public IPutResponse SaveConfig([FromContent] Common.Settings settings)
        {
            try
            {
                Database.Instance.SaveConfigSettings(settings);
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
