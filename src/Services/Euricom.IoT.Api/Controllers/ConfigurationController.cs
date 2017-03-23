using Euricom.IoT.Api.Notifications;
using Euricom.IoT.Common;
using Euricom.IoT.DataLayer;
using Euricom.IoT.FileTransfer;
using Euricom.IoT.Messaging;
using Newtonsoft.Json;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using System;
using System.Threading.Tasks;

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
            Database.Instance.SetValue(table, key, value);
            return new PutResponse(PutResponse.ResponseStatus.OK);
        }

        public IGetResponse GetHardware()
        {
            var hardware = Database.Instance.GetHardware();
            return new GetResponse(GetResponse.ResponseStatus.OK, hardware);
        }
    }
}
