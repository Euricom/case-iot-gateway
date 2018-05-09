using Dropbox.Api;
using Dropbox.Api.Files;
using Euricom.IoT.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Euricom.IoT.DataLayer;
using Euricom.IoT.DataLayer.Interfaces;

namespace Euricom.IoT.Dropbox
{
    public class Dropbox : IDropbox
    {
        private readonly ISettingsRepository _repository;
        private readonly DropboxClientConfig _dropboxClientConfig;
        private readonly Dictionary<string, string> _latestDropboxCursorPerPath;

        public Dropbox(ISettingsRepository repository)
        {
            _repository = repository;
            _dropboxClientConfig = new DropboxClientConfig();
            _latestDropboxCursorPerPath = new Dictionary<string, string>();
        }

        /// <summary>
        /// Get latest file changes (entries) from DropBox folder
        /// </summary>
        /// <param name="cursor"></param>
        public async Task<IList<Metadata>> PollDropboxNewFiles(string path)
        {
            try
            {
                bool recursive = true;
                bool includeMediaInfo = true;
                bool includeDeleted = true;
                bool includeExplicitSharedMembers = true;

                var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromMinutes(5);
                _dropboxClientConfig.HttpClient = httpClient;

                var settings = _repository.Get();
                var dropboxClient = new DropboxClient(settings.DropboxAccessToken, _dropboxClientConfig);

                //The cursor is a sort of GUID which we pass to the dropbox api for getting changed files
                
                // This if happens only once
                if (!_latestDropboxCursorPerPath.ContainsKey(path))
                {
                    var cursor = await dropboxClient.Files.ListFolderGetLatestCursorAsync(path, recursive, includeMediaInfo, includeDeleted, includeExplicitSharedMembers);
                    _latestDropboxCursorPerPath[path] = cursor.Cursor;
                }

                var result = await dropboxClient.Files.ListFolderContinueAsync(_latestDropboxCursorPerPath[path]);
                var entries = result.Entries;

                entries = entries.Where(x => !x.IsDeleted && x.IsFile).ToList();

                var cursorLatest = await dropboxClient.Files.ListFolderGetLatestCursorAsync(path, recursive, includeMediaInfo, includeDeleted, includeExplicitSharedMembers);
                _latestDropboxCursorPerPath[path] = cursorLatest.Cursor;

                return entries;

            }
            catch (HttpException ex)
            {
                Logger.Instance.Error(ex);

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
                    _dropboxClientConfig.HttpClient = httpClient;

                    var settings = _repository.Get();
                    var client = new DropboxClient(settings.DropboxAccessToken, _dropboxClientConfig);

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
                Logger.Instance.Error(ex);

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
