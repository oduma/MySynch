using System;
using System.Collections.Generic;
using MySynch.Core.Interfaces;

namespace MySynch.Tests.Stubs
{
    public class DeffectiveStore<T>:IStore<T>
    {
        public Action<List<T>, string> StoreMethod
        {
            get { return WrongMethod; }
        }

        private void WrongMethod(List<T> arg1, string arg2)
        {
            throw new Exception();
        }

        public Func<string, List<T>> GetMethod
        {
            get { return GoodMethod; }
        }

        private List<T> GoodMethod(string arg)
        {
            return new List<T>();
        }
    }

    public class DeffectiveGetStore<T> : IStore<T>
    {
        public Action<List<T>, string> StoreMethod
        {
            get { return GoodMethod; }
        }

        private void GoodMethod(List<T> arg1, string arg2)
        {
            return;
        }

        public Func<string, List<T>> GetMethod
        {
            get { return WrongMethod; }
        }

        private List<T> WrongMethod(string arg)
        {
            throw new Exception();
        }
    }

}
