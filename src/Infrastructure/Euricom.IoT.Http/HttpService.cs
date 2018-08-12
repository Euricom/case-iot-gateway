using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Euricom.IoT.Interfaces;

namespace Euricom.IoT.Http
{
    public class HttpService: IHttpService
    {
        public async Task<bool> TestConnection(string address, string server)
        {
            HttpWebRequest request = GetRequest(address);
            request.Credentials = CredentialCache.DefaultCredentials;
            HttpWebResponse response = (HttpWebResponse) await request.GetResponseAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (response.Headers["server"].Contains(server))
                {
                    return true;
                }

                throw new Exception("A request to the server succeeded, but couldn't determine if server is motionEye");
            }

            throw new Exception($"Could not get a valid response from {request.RequestUri}");
        }

        public async Task<Stream> GetFile(string url)
        {
            var request = GetRequest(url);

            int i = 0;
            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();

            while (response.StatusCode == HttpStatusCode.OK && response.ContentLength <= 4000 && i < 15)
            {
                await Task.Delay(500);
                i++;

                response = (HttpWebResponse)await request.GetResponseAsync();
            }

            if (response.StatusCode == HttpStatusCode.OK && response.ContentLength > 4000)
            {
                return response.GetResponseStream();
            }

            throw new Exception($"Could not get a valid response from {request.RequestUri}");
        }

        private static HttpWebRequest GetRequest(string address)
        {
            if (String.IsNullOrEmpty(address))
            {
                throw new ArgumentNullException(nameof(address));
            }

            return (HttpWebRequest) (address.Contains("http://") 
                ? WebRequest.Create(address) 
                : WebRequest.Create("http://" + address));
        }
    }
}
