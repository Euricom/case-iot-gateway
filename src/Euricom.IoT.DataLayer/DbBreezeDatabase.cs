using System.Collections.Generic;
using DBreeze;
using DBreeze.Transactions;
using Euricom.IoT.Common;
using Euricom.IoT.DataLayer.Interfaces;
using Newtonsoft.Json;

namespace Euricom.IoT.DataLayer
{
    public class DbBreezeDatabase : IDbBreezeDatabase
    {
        private readonly DBreezeEngine _engine;
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new PrivateResolver()
        };

        public DbBreezeDatabase(DBreezeEngine engine)
        {
            _engine = engine;
        }

        public Transaction GetTransaction()
        {
            return _engine.GetTransaction();
        }

        public void Dispose()
        {
            _engine?.Dispose();
        }

        public T GetValue<T>(string table, string key)
        {
            using (var tran = _engine.GetTransaction())
            {
                var result = tran.Select<string, string>(table, key).Value;
                return JsonConvert.DeserializeObject<T>(result, SerializerSettings);
            }
        }

        public IEnumerable<T> GetValues<T>(string table)
        {
            var values = new List<T>();
            using (var tran = _engine.GetTransaction())
            {
                foreach (var row in tran.SelectForward<string, string>(table))
                {
                    values.Add(JsonConvert.DeserializeObject<T>(row.Value, SerializerSettings));
                }
            }

            return values;
        }

        public void SetValue<T>(string table, string key, T value)
        {
            using (var tran = _engine.GetTransaction())
            {
                tran.Insert(table, key, JsonConvert.SerializeObject(value));
                tran.Commit();
            }
        }
    }
}