using System;
using System.IO;
using MySynch.Core.Interfaces;

namespace MySynch.Core
{
    public class SameSystemCopier:ICopyStrategy
    {
        public bool Copy(string source, string target)
        {
            if(string.IsNullOrEmpty(source))
                throw new ArgumentNullException("source");
            if(string.IsNullOrEmpty(target))
                throw new ArgumentNullException("target");
            if (!File.Exists(source))
                return false;
            try
            {
                Directory.CreateDirectory(target.Substring(0, target.LastIndexOf(@"\")));
                File.Copy(source,target,true);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
