using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MySynch.Core.DataTypes;
using MySynch.Core.Interfaces;

namespace MySynch.Core
{
    public class ChangeApplyer:IChangeApplyer
    {
        private Func<string, string, bool> _copyMethod;

        public bool ApplyChangePackage(ChangePushPackage changePushPackage, string targetRootFolder,Func<string, string, bool> copyMethod)
        {
            if(changePushPackage==null || changePushPackage.ChangePushItems==null || changePushPackage.ChangePushItems.Count<=0)
                throw new ArgumentNullException("changePushPackage");
            if(string.IsNullOrEmpty(targetRootFolder))
                throw new ArgumentNullException("targetRootFolder");
            if(copyMethod==null)
                throw new ArgumentNullException("copyMethod");
            _copyMethod = copyMethod;
            return
                ApplyUpserts(changePushPackage.ChangePushItems.Where(i => i.OperationType == OperationType.Insert || i.OperationType==OperationType.Update),
                             targetRootFolder, changePushPackage.SourceRootName) &&
                ApplyDeletes(changePushPackage.ChangePushItems.Where(i => i.OperationType == OperationType.Delete),
                             targetRootFolder, changePushPackage.SourceRootName);

        }

        private bool ApplyDeletes(IEnumerable<ChangePushItem> deletes, string targetRootFolder, string sourceRootName)
        {
            bool result = true;

            foreach (ChangePushItem delete in deletes)
            {
                var targetFileName = delete.AbsolutePath.Replace(sourceRootName, targetRootFolder);

                if (File.Exists(targetFileName))
                    File.Delete(targetFileName);
                else
                    result = false;
            }
            return result;
        }

        private bool ApplyUpserts(IEnumerable<ChangePushItem> upserts, string targetRootFolder, string sourceRootName)
        {
            bool result = true;
            foreach (ChangePushItem upsert in upserts)
            {
                var tempResult = _copyMethod(upsert.AbsolutePath,
                                             upsert.AbsolutePath.Replace(sourceRootName, targetRootFolder));
                result = result && tempResult;
            }
            return result;
        }
    }
}
