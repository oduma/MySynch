using System;
using System.IO;
using MySynch.Common;
using MySynch.Contracts;
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
                LoggingManager.Debug("Copy from " + source + " to " + target);
                Directory.CreateDirectory(target.Substring(0, target.LastIndexOf(@"\")));
                File.Copy(source,target,true);
                LoggingManager.Debug("Copy Ok.");
                return true;
            }
            catch
            {
                LoggingManager.Debug("Copy Failed.");
                return false;
            }
        }

        public void Initialize(ISourceOfData sourceOfData)
        {
            throw new NotImplementedException();
        }
    }
}
