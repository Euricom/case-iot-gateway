using System;
using System.Collections;
using System.Collections.Generic;
using DBreeze.Transactions;

namespace Euricom.IoT.DataLayer.Interfaces
{
    public interface IDbBreezeDatabase: IDisposable
    {
        Transaction GetTransaction();
        T GetValue<T>(string table, string key);
        IEnumerable<T> GetValues<T>(string table);
        void SetValue<T>(string table, string key, T value);
    }
}