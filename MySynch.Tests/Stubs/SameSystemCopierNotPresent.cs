using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySynch.Core.Interfaces;

namespace MySynch.Tests.Stubs
{
    public class SameSystemCopierNotPresent:ICopyStrategy
    {
        public bool Copy(string source, string target)
        {
            throw new NotImplementedException();
        }
    }
}
