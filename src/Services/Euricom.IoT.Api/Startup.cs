using Euricom.IoT.Api.Controllers;
using Euricom.IoT.DataLayer;
using Restup.Webserver.Http;
using Restup.Webserver.Rest;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using static Euricom.IoT.DataLayer.Database;

namespace Euricom.IoT.Api
{
    public class Startup
    {
        public async void Run()
        {
            // Init DanaLock
            await DanaLock.DanaLock.Instance.Initialize();

            // Init Webserver
            await new WebServer().InitializeWebServer();
        }
    }
}
