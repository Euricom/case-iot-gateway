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

        [UriFormat("/config/update\\?table={table}&key={key}&value={value}")]
        public IPutResponse UpdateConfig(string table, string key, string value)
        {
            //Database.Instance.SetValue(table, key, value);
            return new PutResponse(PutResponse.ResponseStatus.OK);
        }

    }
}
