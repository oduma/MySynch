using System.IO;

namespace MySynch.Core
{
    public static class SameSystemCopier
    {
        public static bool Copy(string source, string target)
        {
            if (!File.Exists(source))
                return false;
            try
            {
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
