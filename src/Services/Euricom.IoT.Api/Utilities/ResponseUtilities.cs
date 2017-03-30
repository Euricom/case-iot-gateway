using Restup.Webserver.Models.Schemas;

namespace Euricom.IoT.Api.Utilities
{
    // TODO Give a better Http status code for bad parameter
    // BadRequestResponse cannot be used (internal class?): https://github.com/tomkuijsten/restup/blob/master/src/WebServer/Models/Schemas/BadRequestResponse.cs
    // Status code can only be NotFound or OK ?
    public static class ResponseUtilities
    {
        public static GetResponse GetResponseOk(string message)
        {
            return new GetResponse(
            GetResponse.ResponseStatus.OK,
            new ResponseData()
            {
                Message = message
            });

        }

        public static PostResponse PostResponseOk(string message)
        {
            return new PostResponse(
            PostResponse.ResponseStatus.Created);
        }

        public static DeleteResponse DeleteResponseOk(string message)
        {
            return new DeleteResponse(
            DeleteResponse.ResponseStatus.OK);
        }

        public static PutResponse PutResponseOk(string message)
        {
            return new PutResponse(
            PutResponse.ResponseStatus.OK,
            new ResponseData()
            {
                Message = message
            });

        }

        public static GetResponse GetResponseFail(string message)
        {
            return new GetResponse(
            GetResponse.ResponseStatus.NotFound,
            new ResponseData()
            {
                Message = message
            });

        }

        public static PostResponse PostResponseFail(string message)
        {
            return new PostResponse(
            PostResponse.ResponseStatus.Conflict);
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
