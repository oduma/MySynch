using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MySynch.Common;
using MySynch.Contracts;
using MySynch.Contracts.Messages;

namespace MySynch.Core
{
    public class ChangeApplyer:IChangeApplyer
    {
        private Func<string, string, bool> _copyMethod;

        public bool ApplyChangePackage(ChangePushPackage changePushPackage, string targetRootFolder,Func<string, string, bool> copyMethod)
        {
            LoggingManager.Debug("Trying to apply some changes to: " + targetRootFolder);
            if(changePushPackage==null || changePushPackage.ChangePushItems==null || changePushPackage.ChangePushItems.Count<=0)
                throw new ArgumentNullException("changePushPackage");
            if(string.IsNullOrEmpty(targetRootFolder))
                throw new ArgumentNullException("targetRootFolder");
            if(copyMethod==null)
                throw new ArgumentNullException("copyMethod");
            _copyMethod = copyMethod;
            var response=ApplyUpserts(changePushPackage.ChangePushItems.Where(i => i.OperationType == OperationType.Insert || i.OperationType==OperationType.Update),
                             targetRootFolder, changePushPackage.SourceRootName) &&
                ApplyDeletes(changePushPackage.ChangePushItems.Where(i => i.OperationType == OperationType.Delete),
                             targetRootFolder, changePushPackage.SourceRootName);
            LoggingManager.Debug("Result of applying changes is: " +response);
            return response;

        }

        private bool ApplyDeletes(IEnumerable<ChangePushItem> deletes, string targetRootFolder, string sourceRootName)
        {
            LoggingManager.Debug("Applying deletes to " + targetRootFolder);

            bool result = true;

            foreach (ChangePushItem delete in deletes)
            {
                var targetFileName = delete.AbsolutePath.Replace(sourceRootName, targetRootFolder);

                if (File.Exists(targetFileName))
                    File.Delete(targetFileName);
                else
                    result = false;
            }
            LoggingManager.Debug("Apply deletes returns " + result);

            return result;
        }

        private bool ApplyUpserts(IEnumerable<ChangePushItem> upserts, string targetRootFolder, string sourceRootName)
        {
            LoggingManager.Debug("Applying upserts from " + sourceRootName + " to " +targetRootFolder);
            bool result = true;
            foreach (ChangePushItem upsert in upserts)
            {
                var tempResult = _copyMethod(upsert.AbsolutePath,
                                             upsert.AbsolutePath.Replace(sourceRootName, targetRootFolder));
                result = result && tempResult;
            }
            LoggingManager.Debug("Apply upserts returns "+ result);
            return result;
        }

        public string MachineName
        {
            get { return Environment.MachineName; }
        }

        public HeartbeatResponse GetHeartbeat()
        {
            LoggingManager.Debug("GetHeartbeat will return true.");
            return new HeartbeatResponse {Status = true};
        }
    }
}
