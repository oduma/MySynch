using System;
using System.Collections.Generic;

namespace MySynch.Core.Interfaces
{
    public interface IStore<T>
    {
        Action<List<T>, string> StoreMethod { get; }

        Func<string,List<T>> GetMethod { get; }
    }
}
