using Restup.Webserver.Models.Schemas;

namespace Euricom.IoT.Api.Utilities
{
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

        public static DeleteResponse DeleteResponseOk(string message = null)
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
    }
}