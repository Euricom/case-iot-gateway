using DBreeze;
using DBreeze.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euricom.IoT.DataLayer
{
    public class Database
    {
        public static DBreeze.DBreezeEngine engine = null;

        public Database()
        {
        }

        public void InitDB()
        {
            if (engine == null)
            {
                engine = new DBreezeEngine(new DBreezeConfiguration { DBreezeDataFolderName = @"C:\temp" });

                //Setting default serializer for DBreeze
                //DBreeze.Utils.CustomSerializator.ByteArraySerializator = ProtobufSerializer.SerializeProtobuf;
                //DBreeze.Utils.CustomSerializator.ByteArrayDeSerializator = ProtobufSerializer.DeserializeProtobuf;
            }

            using (var tran = engine.GetTransaction())
            {
                tran.Insert<int, int>("t1", 1, 1);
                tran.Insert<int, int>("t1", 1, 2);
                tran.Commit();
            }
        }
    }
}
