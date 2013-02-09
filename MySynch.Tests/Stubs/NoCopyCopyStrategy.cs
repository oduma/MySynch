using System;
using MySynch.Contracts;
using MySynch.Core.Interfaces;

namespace MySynch.Tests.Stubs
{
    public class NoCopyCopyStrategy:ICopyStrategy
    {
        public bool Copy(string source, string target)
        {
            Console.WriteLine("Copying from {0} to {1}",source,target);
            return true;
        }

        public void Initialize(ISourceOfData sourceOfData)
        {
            return;
        }
    }
}
