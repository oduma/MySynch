using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using MySynch.Common;
using MySynch.Contracts;
using MySynch.Contracts.Messages;
using MySynch.Core.Interfaces;
using MySynch.Proxies;

namespace MySynch.Core
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Subscriber:IChangeSubscriber
    {
        private Func<string, string, bool> _copyMethod;
        private string _targetRootFolder;


        public bool ApplyChangePackage(ChangePushPackage changePushPackage,string sourceOfDataEndpointName)
        {
            LoggingManager.Debug("Trying to apply some changes to: " + _targetRootFolder);
            if(changePushPackage==null || changePushPackage.ChangePushItems==null || changePushPackage.ChangePushItems.Count<=0)
            {
                LoggingManager.Debug("Nothing to apply.");
                return false;
            }
            ISourceOfData sourceOfData;
            if (string.IsNullOrEmpty(sourceOfDataEndpointName))
                sourceOfData = new LocalSourceOfData();
            else
            {
                sourceOfData = new SourceOfDataClient();
                ((SourceOfDataClient)sourceOfData).InitiateUsingEndpoint(sourceOfDataEndpointName);
            }
            CopyStrategy copyStrategy= new CopyStrategy();
            copyStrategy.Initialize(sourceOfData);
            return TryApplyChanges(changePushPackage,copyStrategy);
        }

        public string GetTargetRootFolder()
        {
            return _targetRootFolder;
        }

        internal bool TryApplyChanges(ChangePushPackage changePushPackage, ICopyStrategy copyStrategy)
        {
            _copyMethod = copyStrategy.Copy;
            var response = ApplyUpserts(changePushPackage.ChangePushItems.Where(i => i.OperationType == OperationType.Insert || i.OperationType == OperationType.Update),
                                      _targetRootFolder, changePushPackage.SourceRootName) &&
                         ApplyDeletes(changePushPackage.ChangePushItems.Where(i => i.OperationType == OperationType.Delete),
                                      _targetRootFolder, changePushPackage.SourceRootName);
            LoggingManager.Debug("Result of applying changes is: " +response);
            return response;
        }

        public void Initialize(string targetRootFolder)
        {
            if (string.IsNullOrEmpty(targetRootFolder))
                throw new ArgumentNullException("targetRootFolder");
            _targetRootFolder = targetRootFolder;
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
