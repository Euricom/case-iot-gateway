using Dropbox.Api;
using Dropbox.Api.Files;
using Euricom.IoT.Common.Secrets;
using Euricom.IoT.Managers.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Euricom.IoT.Managers
{
    public class DropboxManager : IDropboxManager
    {
        private string _accessToken = Secrets.DROPBOX_ACCESS_TOKEN;
        private DropboxClientConfig _config;
        private string _cursor;

        public DropboxManager()
        {
            _config = new DropboxClientConfig("SimpleTestApp");
        }

        /// <summary>
        /// Get latest file changes (entries) from DropBox folder
        /// </summary>
        /// <param name="cursor"></param>
        public async Task<IList<Metadata>> PollDropboxNewFiles()
        {
            try
            {
                string path = @"/camera";
                bool recursive = true;
                bool includeMediaInfo = true;
                bool includeDeleted = true;
                bool includeExplicitSharedMembers = true;

                var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromMinutes(5);
                _config.HttpClient = httpClient;

                var dropboxClient = new DropboxClient(_accessToken, _config);

                if (String.IsNullOrEmpty(_cursor))
                {
                    var cursor = await dropboxClient.Files.ListFolderGetLatestCursorAsync(path, recursive, includeMediaInfo, includeDeleted, includeExplicitSharedMembers);
                    _cursor = cursor.Cursor;
                }

                var result = await dropboxClient.Files.ListFolderContinueAsync(_cursor);
                var entries = result.Entries;

                entries = entries.Where(x => !x.IsDeleted && x.IsFile).ToList();

                var cursor2 = await dropboxClient.Files.ListFolderGetLatestCursorAsync(path, recursive, includeMediaInfo, includeDeleted, includeExplicitSharedMembers);
                _cursor = cursor2.Cursor;

                return entries;

            }
            catch (HttpException ex)
            {
                Debug.WriteLine("Exception reported from RPC layer");
                Debug.WriteLine("    Status code: {0}", ex.StatusCode);
                Debug.WriteLine("    Message    : {0}", ex.Message);
                if (ex.RequestUri != null)
                {
                    Debug.WriteLine("    Request uri: {0}", ex.RequestUri);
                }

                throw;
            }
        }

        public async Task<Dictionary<string, byte[]>> DownloadFiles(IList<Metadata> entries)
        {
            try
            {
                Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();
                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromMinutes(5);
                    _config.HttpClient = httpClient;

                    var client = new DropboxClient(_accessToken, _config);

                    foreach (var entry in entries)
                    {
                        var file = await client.Files.DownloadAsync(entry.PathLower);
                        var fileBytes = await file.GetContentAsByteArrayAsync();
                        files.Add(entry.Name, fileBytes);
                    }
                }

                return files;
            }
            catch (HttpException ex)
            {
                Debug.WriteLine("Exception reported from RPC layer");
                Debug.WriteLine("    Status code: {0}", ex.StatusCode);
                Debug.WriteLine("    Message    : {0}", ex.Message);
                if (ex.RequestUri != null)
                {
                    Debug.WriteLine("    Request uri: {0}", ex.RequestUri);
                }

                throw;
            }
        }

        private async Task GetCurrentAccount(DropboxClient client)
        {
            var full = await client.Users.GetCurrentAccountAsync();

            Debug.WriteLine("Account id    : {0}", full.AccountId);
            Debug.WriteLine("Country       : {0}", full.Country);
            Debug.WriteLine("Email         : {0}", full.Email);
            Debug.WriteLine("Is paired     : {0}", full.IsPaired ? "Yes" : "No");
            Debug.WriteLine("Locale        : {0}", full.Locale);
            Debug.WriteLine("Name");
            Debug.WriteLine("  Display  : {0}", full.Name.DisplayName);
            Debug.WriteLine("  Familiar : {0}", full.Name.FamiliarName);
            Debug.WriteLine("  Given    : {0}", full.Name.GivenName);
            Debug.WriteLine("  Surname  : {0}", full.Name.Surname);
            Debug.WriteLine("Referral link : {0}", full.ReferralLink);
        }

    }
}
