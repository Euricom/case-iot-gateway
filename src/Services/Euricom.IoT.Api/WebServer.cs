using Restup.Webserver.Http;
using Restup.Webserver.Rest;
using System;
using System.Threading.Tasks;
using Restup.Webserver.File;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using Autofac;
using Euricom.IoT.Api.Authentication;
using Euricom.IoT.Api.Utilities;
using Restup.Webserver.Attributes;

namespace Euricom.IoT.Api
{
    public class WebServer : IDisposable
    {
        private readonly ILifetimeScope _lifetimeScope;
        private readonly JwtAuthenticationProvider _authProvider;
        private HttpServer _httpServer;

        public WebServer(ILifetimeScope lifetimeScope, JwtAuthenticationProvider authProvider)
        {
            _lifetimeScope = lifetimeScope;
            _authProvider = authProvider;
        }

        public async Task InitializeWebServer()
        {
            var restRouteHandler = new DependencyResolverRouteHandler(type => _lifetimeScope.Resolve(type), _authProvider);

            // Register controllers
            foreach (Type type in GetType().GetTypeInfo().Assembly.GetTypes())
            {
                var attribute = type.GetTypeInfo().GetCustomAttribute<RestControllerAttribute>();
                if (attribute != null)
                {
                   restRouteHandler.RegisterController(type);
                }
            }

            var configuration = new HttpServerConfiguration()
              .ListenOnPort(8800)
              .RegisterRoute("api", restRouteHandler)
              .RegisterRoute("", new StaticFileRouteHandler(@"Euricom.IoT.UI.WebAdministration\Web"))
              .EnableCors()
              .RegisterCustomMessageInspector(new LoggingMessageInspector());

            _httpServer = new HttpServer(configuration);
            await _httpServer.StartServerAsync();

            Debug.WriteLine("Restup Web Server initialized, listening on default port 8800");
        }

        public static MethodInfo GetMethod<T>(Expression<Action<T>> expr)
        {
            return ((MethodCallExpression)expr.Body)
                .Method
                .GetGenericMethodDefinition();
        }

        public void Dispose()
        {
            _httpServer.StopServer();
            ((IDisposable)_httpServer)?.Dispose();
        }
    }
}
