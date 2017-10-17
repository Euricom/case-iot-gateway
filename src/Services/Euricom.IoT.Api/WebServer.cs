using Restup.Webserver.Http;
using Restup.Webserver.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Restup.Webserver.File;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using Autofac;
using Restup.WebServer.Http;
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
            var restRouteHandler = new RestRouteHandler(_authProvider);

            // Register controllers
            foreach (Type type in GetType().GetTypeInfo().Assembly.GetTypes())
            {
                var attribute = type.GetTypeInfo().GetCustomAttribute<RestControllerAttribute>();
                if (attribute != null)
                {
                    var @params = new List<object>();

                    var constructor = type.GetConstructors().FirstOrDefault();
                    if (constructor != null)
                    {
                        var parameters = constructor.GetParameters();
                        foreach (var parameterInfo in parameters)
                        {
                            @params.Add(_lifetimeScope.Resolve(parameterInfo.ParameterType));
                        }
                    }

                    try
                    {
                        MethodInfo method = @params.Any()
                            ? GetMethod<RestRouteHandler>(x =>
                                x.RegisterController<object>(new object[] { @params.ToArray() }))
                            : GetMethod<RestRouteHandler>(x => x.RegisterController<object>());
                        MethodInfo genericMethod = method.MakeGenericMethod(type);
                        genericMethod.Invoke(restRouteHandler, new object[]{ @params.ToArray() });
                    }
                    catch (Exception e)
                    {
                        
                    }
                }
            }

            //restRouteHandler.RegisterController<ConfigurationController>();
            //restRouteHandler.RegisterController<DevicesController>();
            //restRouteHandler.RegisterController<CameraController>();
            //restRouteHandler.RegisterController<DanaLockController>();
            //restRouteHandler.RegisterController<LazyBoneController>();
            //restRouteHandler.RegisterController<WallMountSwitchController>();
            //restRouteHandler.RegisterController<LogController>();
            //restRouteHandler.RegisterController<SecurityController>();

            var configuration = new HttpServerConfiguration()
              .ListenOnPort(8800)
              .RegisterRoute("api", restRouteHandler)
              .RegisterRoute(new StaticFileRouteHandler(@"Euricom.IoT.UI.WebAdministration\Web"))
              .EnableCors();

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
