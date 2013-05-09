using System;
using System.Collections.Generic;

namespace MySynch.Common.Serialization
{
    public interface IStore<T>
    {
        Action<List<T>, string> StoreMethod { get; }

        Func<string,List<T>> GetMethod { get; }
    }
}
