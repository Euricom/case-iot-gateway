using System;
using DBreeze.Transactions;

namespace Euricom.IoT.DataLayer.Interfaces
{
    public interface IDbBreezeDatabase: IDisposable
    {
        Transaction GetTransaction();
        T GetValue<T>(string table, string key);
        void SetValue<T>(string table, string key, T value);
    }
}