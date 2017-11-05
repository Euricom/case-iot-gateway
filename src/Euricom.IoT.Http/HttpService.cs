using System;
using System.Net;
using System.Threading.Tasks;
using Euricom.IoT.Http.Interfaces;

namespace Euricom.IoT.Http
{
    public class HttpService: IHttpService
    {
        public async Task<bool> TestConnection(string address, string server)
        {
            HttpWebRequest request = GetAddress(address);
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

        private static HttpWebRequest GetAddress(string address)
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
