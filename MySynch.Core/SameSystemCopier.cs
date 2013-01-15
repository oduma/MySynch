using System;
using System.IO;

namespace MySynch.Core
{
    public static class SameSystemCopier
    {
        public static bool Copy(string source, string target)
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
