using System;
using Euricom.IoT.Common.Exceptions;
using Euricom.IoT.Logging;
using Restup.HttpMessage;
using Restup.HttpMessage.Models.Schemas;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using Restup.Webserver.Rest;

namespace Euricom.IoT.Api.Utilities
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public IRestResponse Handle(IHttpServerRequest request, Exception exception)
        {
            Logger.Instance.Error(exception, request.ToMessageString());

            if (exception is NotFoundException)
            {
                return new RestResponse(HttpResponseStatus.NotFound);
            }

            if (exception is NotAllowedException)
            {
                return new RestResponse(HttpResponseStatus.Forbidden);
            }

            if (exception is UnauthorizedException)
            {
                return new RestResponse(HttpResponseStatus.Forbidden);
            }

            if (exception is ArgumentException)
            {
                return new RestResponse(HttpResponseStatus.BadRequest);
            }

            if (exception is InvalidOperationException)
            {
                return new RestResponse(HttpResponseStatus.BadRequest);
            }

            return null;
        }
    }
}
