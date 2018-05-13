using Euricom.IoT.Logging;
using Euricom.IoT.Models.Logging;
using Restup.HttpMessage;
using Restup.Webserver.Http;

namespace Euricom.IoT.Api.Utilities
{
    public class LoggingMessageInspector : IHttpMessageInspector
    {
        public BeforeHandleRequestResult BeforeHandleRequest(IHttpServerRequest request)
        {
            return null;
        }

        public AfterHandleRequestResult AfterHandleRequest(IHttpServerRequest request, HttpServerResponse httpResponse)
        {
            if (Logger.Instance.IsEnabled(LogLevel.Verbose))
            {
                Logger.Instance.Verbose(request.ToMessageString());
            }

            return null;
        }
    }
}