using DBreeze;
using DBreeze.Transactions;
using Euricom.IoT.DataLayer.Interfaces;

namespace Euricom.IoT.DataLayer
{
    public class DbBreezeDatabase: IDbBreezeDatabase
    {
        private readonly DBreezeEngine _engine;

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
                return tran.Select<string, T>(table, key).Value;
            }
        }

        public void SetValue<T>(string table, string key, T value)
        {
            using (var tran = _engine.GetTransaction())
            {
                tran.Insert(table, key, value);
                tran.Commit();
            }
        }
    }
}