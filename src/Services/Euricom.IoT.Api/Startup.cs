using Euricom.IoT.Devices.DanaLock;

namespace Euricom.IoT.Api
{
    public class Startup
    {
        public async void Run()
        {
            // Init DanaLock
            await DanaLock.Instance.Initialize();

            // Init Webserver
            await new WebServer().InitializeWebServer();
        }
    }
}
