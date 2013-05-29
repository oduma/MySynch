using System;
using System.IO;
using MySynch.Contracts;
using MySynch.Contracts.Messages;
using Sciendo.Common.Logging;

namespace MySynch.Core.Subscriber
{
    public class CopyStrategy
    {
        private IPublisher _publisher;

        public virtual bool Copy(string source, string target)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(target))
                throw new ArgumentNullException("target");
            string backupFileName = Path.GetDirectoryName(target) + @"\" + "cpsb" + Guid.NewGuid().ToString();
            string temporaryTarget = Path.GetDirectoryName(target) + @"\" + "cpst" + Guid.NewGuid().ToString();
            if (File.Exists(target))
            {
                File.Copy(target, backupFileName);
            }
            try
            {
                if (CopytoTemporaryFile(source, temporaryTarget))
                {
                    if (File.Exists(target))
                        File.Delete(target);
                    File.Copy(temporaryTarget, target, true);
                    File.Delete(temporaryTarget);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                LoggingManager.LogSciendoSystemError(ex);
                if(File.Exists(backupFileName))
                    File.Copy(backupFileName, target,true);
                return false;
            }
            finally
            {
                if(File.Exists(backupFileName))
                    File.Delete(backupFileName);
            }
        }

        private bool CopytoTemporaryFile(string source, string temporaryTarget)
        {
            if (!Directory.Exists(Path.GetDirectoryName(temporaryTarget)))
                Directory.CreateDirectory(Path.GetDirectoryName(temporaryTarget));

            if (_publisher == null)
                return false;

            var response = _publisher.GetData(new GetDataRequest { FileName = source });
            using (var stream = File.Create(temporaryTarget))
            {
                stream.Write(response.Data, 0, response.Data.Length);
                stream.Flush();
            }
            return true;
        }

        public void Initialize(IPublisher publisher)
        {
            _publisher = publisher;
        }
    }
}
