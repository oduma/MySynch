using System;
using System.Collections.Generic;
using MySynch.Core.Interfaces;

namespace MySynch.Core.Broker
{
    public class MemoryStore<T>:IStore<T> where T:class
    {
        private SortedList<string, List<T>> _collection;

        public MemoryStore()
        {
            _collection=new SortedList<string, List<T>>();
        }

        public Action<List<T>, string> StoreMethod
        {
            get { return PlaceInMemory; }
        }

        private void PlaceInMemory(List<T> values, string key)
        {
            if (_collection.ContainsKey(key))
                _collection[key] = values;
            else
                _collection.Add(key,values);
        }

        public Func<string, List<T>> GetMethod
        {
            get { return GetFromMemory; }
        }

        private List<T> GetFromMemory(string key)
        {
            if (_collection.ContainsKey(key))
                return _collection[key];
            return new List<T>();
        }
    }
}
