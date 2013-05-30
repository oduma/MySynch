using System;
using System.Collections.Generic;
using MySynch.Core.Interfaces;
using Sciendo.Common.Serialization;

namespace MySynch.Core.Broker
{
    public class FileSystemStore<T>:IStore<T> where T:class
    {
        public Action<List<T>, string> StoreMethod
        {
            get { return Serializer.SerializeToFile; }
        }

        public Func<string,List<T>> GetMethod
        {
            get { return Serializer.DeserializeFromFile<T>; }
        }
    }
}
