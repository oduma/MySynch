using System;
using System.IO;
using MySynch.Common;
using MySynch.Contracts;
using MySynch.Contracts.Messages;
using MySynch.Core.Interfaces;

namespace MySynch.Core
{
    public class RemoteSystemCopier:ICopyStrategy
    {
        private ISourceOfData _sourceOfData;

        public bool Copy(string source, string target)
        {
            if(string.IsNullOrEmpty(source))
                throw new ArgumentNullException("source");
            if(string.IsNullOrEmpty(target))
                throw new ArgumentNullException("target");
            if (_sourceOfData == null)
                throw new SourceOfDataSetupException(source,"Not initialize.");

            string backupFileName = Path.GetDirectoryName(target) + @"\" + Guid.NewGuid().ToString();
            string temporaryTarget = Path.GetDirectoryName(target) + @"\" + Guid.NewGuid().ToString();
            if(File.Exists(target))
            {
                File.Copy(target,backupFileName);
            }
            try
            {
            var response = _sourceOfData.GetData(new RemoteRequest {FileName = source});
            using (var stream = File.Create(temporaryTarget))
            {
                stream.Write(response.Data,0,response.Data.Length);
                stream.Flush();
            }
            if (File.Exists(target))
                File.Delete(target);
            File.Copy(temporaryTarget,target);
                File.Delete(temporaryTarget);
                return true;
            }
            catch (Exception ex)
            {
                LoggingManager.LogMySynchSystemError(ex);
                File.Copy(backupFileName,target);
                return false;
            }
            finally
            {
                File.Delete(backupFileName);
            }
        }

        public void Initialize(ISourceOfData sourceOfData)
        {
            _sourceOfData = sourceOfData;
        }
    }
}
