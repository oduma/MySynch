using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using MySynch.Common;
using MySynch.Contracts;
using MySynch.Contracts.Messages;
using MySynch.Core.Interfaces;
using MySynch.Core.Publisher;
using MySynch.Proxies;

namespace MySynch.Core.Subscriber
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Subscriber:IChangeSubscriber
    {
        private ICopyStrategy _copyStrategy;
        private string _targetRootFolder;

        internal ICopyStrategy CopyStrategy
        {
            set { _copyStrategy = value; }
        }
        public ApplyChangePackageResponse ApplyChangePackage(PublishPackageRequestResponse publishPackageRequestResponse)
        {
            LoggingManager.Debug("Trying to apply some changes to: " + _targetRootFolder);
            if(publishPackageRequestResponse==null || publishPackageRequestResponse.ChangePushItems==null || publishPackageRequestResponse.ChangePushItems.Count<=0)
            {
                LoggingManager.Debug("Nothing to apply.");
                return new ApplyChangePackageResponse {Status = false};
            }
            if (_copyStrategy == null)
            {
                throw new SourceOfDataSetupException("","No source of data established");
            }
            return new ApplyChangePackageResponse {Status = TryApplyChanges(publishPackageRequestResponse)};
        }

        public GetTargetFolderResponse GetTargetRootFolder()
        {
            return new GetTargetFolderResponse {RootFolder = _targetRootFolder};
        }

        public TryOpenChannelResponse TryOpenChannel(TryOpenChannelRequest request)
        {
            if (request == null)
                request = new TryOpenChannelRequest {SourceOfDataPort = 0};
            LoggingManager.Debug("Trying to open channel to: " + request.SourceOfDataPort);
            if (_copyStrategy != null)
            {
                LoggingManager.Debug("Channel already opened.");
                return new TryOpenChannelResponse {Status = true};
            }
            try
            {
                ISourceOfData sourceOfData;
                if (request.SourceOfDataPort==0)
                    sourceOfData = new LocalSourceOfData();
                else
                {
                    sourceOfData = new SourceOfDataClient();
                    ((SourceOfDataClient)sourceOfData).InitiateUsingPort(request.SourceOfDataPort);
                }
                _copyStrategy = new CopyStrategy();
                _copyStrategy.Initialize(sourceOfData);
                LoggingManager.Debug("Channel opened.");
                return new TryOpenChannelResponse{Status=true};

            }
            catch (Exception ex)
            {
                LoggingManager.LogMySynchSystemError(ex);
                return new TryOpenChannelResponse{Status=false};
            }
        }

        internal bool TryApplyChanges(PublishPackageRequestResponse publishPackageRequestResponse)
        {
            var response = ApplyUpserts(publishPackageRequestResponse.ChangePushItems.Where(i => i.OperationType == OperationType.Insert || i.OperationType == OperationType.Update),
                                      _targetRootFolder, publishPackageRequestResponse.SourceRootName) &&
                         ApplyDeletes(publishPackageRequestResponse.ChangePushItems.Where(i => i.OperationType == OperationType.Delete),
                                      _targetRootFolder, publishPackageRequestResponse.SourceRootName);
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
                var tempResult = _copyStrategy.Copy(upsert.AbsolutePath,Path.Combine(targetRootFolder,upsert.AbsolutePath.Replace(sourceRootName,"")));
                result = result && tempResult;
            }
            LoggingManager.Debug("Apply upserts returns "+ result);
            return result;
        }

        public string MachineName
        {
            get { return Environment.MachineName; }
        }

        public GetHeartbeatResponse GetHeartbeat()
        {
            LoggingManager.Debug("GetHeartbeat will return true.");
            return new GetHeartbeatResponse {Status = true};
        }
    }
}
