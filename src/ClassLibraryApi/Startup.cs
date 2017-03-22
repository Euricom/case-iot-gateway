
using Restup.Webserver.Http;
using Windows.ApplicationModel.Background;

namespace ClassLibraryApi
{
    public class Startup
    {
        private HttpServer _httpServer;
        private BackgroundTaskDeferral _deferral;
        //private DeviceConfigurations _deviceConfigs;

        public async void Run()
        {
            // Read configuration
            //_deviceConfigs = new ConfigReader().ReadConfiguration();

            // Init DanaLock
            //await InitDanaLock();

            // Init Webserver
            await new WebServer().InitializeWebServer();
        }

        //private async Task InitDanaLock()
        //{
        //    //await DanaLock.Instance.Initialize();
        //}
    }
}
