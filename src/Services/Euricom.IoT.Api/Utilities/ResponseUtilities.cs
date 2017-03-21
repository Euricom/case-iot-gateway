using Restup.Webserver.Models.Schemas;

namespace Euricom.IoT.Api.Utilities
{
    // TODO Give a better Http status code for bad parameter
    // BadRequestResponse cannot be used (internal class?): https://github.com/tomkuijsten/restup/blob/master/src/WebServer/Models/Schemas/BadRequestResponse.cs
    // Status code can only be NotFound or OK ?
    public static class ResponseUtilities
    {
        public static GetResponse ResponseOk(string message)
        {
            return new GetResponse(
            GetResponse.ResponseStatus.OK,
            new ResponseData()
            {
                Message = message
            });

        }

        public static GetResponse ResponseFail(string message)
        {
            return new GetResponse(
            GetResponse.ResponseStatus.NotFound,
            new ResponseData()
            {
                Message = message
            });

        }
    }
}
