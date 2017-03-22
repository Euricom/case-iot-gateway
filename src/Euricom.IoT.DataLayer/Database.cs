using DBreeze;
using DBreeze.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Euricom.IoT.DataLayer
{
    public class Database : IDisposable
    {
        private static readonly Database _instance = new Database();

        public static DBreeze.DBreezeEngine _engine = null;

        private Database()
        {
            InitDB();
        }

        public static Database Instance
        {
            get
            {
                return _instance;
            }
        }

        private async void InitDB()
        {
            if (_engine == null)
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;

                //test write to local folder
                //StorageFile sampleFile = await localFolder.CreateFileAsync("dataFile.txt");
                //await FileIO.WriteTextAsync(sampleFile, "My text");

                //test read the first line of dataFile.txt in LocalFolder and store it in a String
                //StorageFile sampleFile = await localFolder.GetFileAsync("dataFile.txt");
                //String fileContent = await FileIO.ReadTextAsync(sampleFile);

                _engine = new DBreezeEngine(new DBreezeConfiguration { DBreezeDataFolderName = localFolder.Path });
            }

            try
            {
                using (var tran = _engine.GetTransaction())
                {
                    tran.Insert<string, string>("t1", "azure-admin", "admin");
                    tran.Insert<string, string>("t1", "azure-pass", "secret-password");
                    tran.Commit();

                    var row = tran.Select<string, string>("t1", "azure-pass").Value;
                }
            }
            catch (Exception ex)
            {
                //TODO add logging to file ?
                throw;
            }
        }

        public void Dispose()
        {
            if (_engine != null)
            {
                _engine.Dispose();
            }
        }
    }
}
