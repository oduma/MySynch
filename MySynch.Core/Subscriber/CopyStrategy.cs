﻿using System;
using System.IO;
using MySynch.Common;
using MySynch.Common.Logging;
using MySynch.Contracts;
using MySynch.Contracts.Messages;
using MySynch.Core.Interfaces;

namespace MySynch.Core.Subscriber
{
    public class CopyStrategy:ICopyStrategy
    {
        private ISourceOfData _sourceOfData;

        public bool Copy(string source, string target)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(target))
                throw new ArgumentNullException("target");
            string backupFileName = Path.GetDirectoryName(target) + @"\" + Guid.NewGuid().ToString();
            string temporaryTarget = Path.GetDirectoryName(target) + @"\" + Guid.NewGuid().ToString();
            if (File.Exists(target))
            {
                File.Copy(target, backupFileName);
            }
            try
            {
                CopytoTemporaryFile(source, temporaryTarget);
                if (File.Exists(target))
                    File.Delete(target);
                if (File.Exists(temporaryTarget))
                {
                    File.Copy(temporaryTarget, target,true);
                    File.Delete(temporaryTarget);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                LoggingManager.LogMySynchSystemError(ex);
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

        private void CopytoTemporaryFile(string source, string temporaryTarget)
        {
            if (!Directory.Exists(Path.GetDirectoryName(temporaryTarget)))
                Directory.CreateDirectory(Path.GetDirectoryName(temporaryTarget));

            if (_sourceOfData==null || _sourceOfData.GetType().ToString()=="MySynch.Core.LocalSourceOfData")
                if(File.Exists(source))
                {
                    File.Copy(source, temporaryTarget);
                    return;
                }
            var response = _sourceOfData.GetData(new RemoteRequest { FileName = source });
            using (var stream = File.Create(temporaryTarget))
            {
                stream.Write(response.Data, 0, response.Data.Length);
                stream.Flush();
            }
        }

        public void Initialize(ISourceOfData sourceOfData)
        {
            _sourceOfData = sourceOfData;
        }
    }
}
