using Restup.Webserver.Models.Schemas;

namespace Euricom.IoT.Api.Utilities
{
    // TODO Give a better Http status code for bad parameter
    // BadRequestResponse cannot be used (internal class?): https://github.com/tomkuijsten/restup/blob/master/src/WebServer/Models/Schemas/BadRequestResponse.cs
    // Status code can only be NotFound or OK ?
    public static class ResponseUtilities
    {
        public static GetResponse GetResponseOk(object data)
        {
            return new GetResponse(
            GetResponse.ResponseStatus.OK,
            data
            );
        }

        public static PostResponse PostResponseOk(object data)
        {
            return new PostResponse(
            PostResponse.ResponseStatus.Created,
            "",
            data);
        }

        public static DeleteResponse DeleteResponseOk(string message)
        {
            return new DeleteResponse(
            DeleteResponse.ResponseStatus.OK);
        }

        public static PutResponse PutResponseOk(object data)
        {
            return new PutResponse(
            PutResponse.ResponseStatus.OK,
            data);
        }

        public static GetResponse GetResponseFail(object data)
        {
            return new GetResponse(
            GetResponse.ResponseStatus.NotFound,
            data);

        }

        public static PostResponse PostResponseFail(string message)
        {
            return new PostResponse(
            PostResponse.ResponseStatus.Conflict,
            "",
            new ResponseData()
            {
                Message = message
            });
        }

        public static DeleteResponse DeleteResponseFail(string message)
        {
            return new DeleteResponse(
            DeleteResponse.ResponseStatus.NotFound);
        }

        public static PutResponse PutResponseFail(string message)
        {
            return new PutResponse(
            PutResponse.ResponseStatus.NotFound,
            new ResponseData()
            {
                Message = message
            });
        }
    }
}
