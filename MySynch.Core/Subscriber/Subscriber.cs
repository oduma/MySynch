using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using MySynch.Common;
using MySynch.Common.Logging;
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

        public ApplyChangePushItemResponse ApplyChangePushItem(ApplyChangePushItemRequest applyChangePushItemRequest)
        {
            LoggingManager.Debug("Trying to apply a change to: " + _targetRootFolder);

            if (applyChangePushItemRequest == null || applyChangePushItemRequest.ChangePushItem == null)
            {
                throw new ArgumentNullException("applyChangePushItemRequest");  
            }
            if (string.IsNullOrEmpty(applyChangePushItemRequest.ChangePushItem.AbsolutePath))
                return new ApplyChangePushItemResponse
                           {ChangePushItem = applyChangePushItemRequest.ChangePushItem, Success = false};
            if (_copyStrategy == null)
            {
                throw new SourceOfDataSetupException("", "No source of data established");
            }
            return TryApplyChange(applyChangePushItemRequest);
        }

        internal ApplyChangePushItemResponse TryApplyChange(ApplyChangePushItemRequest applyChangePushItemRequest)
        {
            return (applyChangePushItemRequest.ChangePushItem.OperationType != OperationType.Delete)
                ? ApplyUpsert(applyChangePushItemRequest, _targetRootFolder)
                : ApplyDelete(applyChangePushItemRequest, _targetRootFolder);
        }

        private ApplyChangePushItemResponse ApplyDelete(ApplyChangePushItemRequest applyChangePushItemRequest, string targetRootFolder)
        {
            LoggingManager.Debug("Applying delete to " + targetRootFolder);

            var response = new ApplyChangePushItemResponse
                               {
                                   ChangePushItem =
                                       new ChangePushItem
                                           {
                                               AbsolutePath =
                                                   applyChangePushItemRequest.ChangePushItem.AbsolutePath.Replace(
                                                       applyChangePushItemRequest.SourceRootName, targetRootFolder),
                                               OperationType = OperationType.Delete
                                           },
                                   Success = false
                               };

            try
            {
                if (File.Exists(response.ChangePushItem.AbsolutePath))
                    File.Delete(response.ChangePushItem.AbsolutePath);
                response.Success = true;
            }
            catch (Exception ex)
            {
                LoggingManager.LogMySynchSystemError(ex);
                response.Success = false;
            }
            LoggingManager.Debug("Apply deletes returns " + response.Success);

            return response;
        }

        private ApplyChangePushItemResponse ApplyUpsert(ApplyChangePushItemRequest applyChangePushItemRequest, string targetRootFolder)
        {
            LoggingManager.Debug("Applying upsert from " + applyChangePushItemRequest.SourceRootName + " to " + targetRootFolder);
            return new ApplyChangePushItemResponse
                       {
                           ChangePushItem = new ChangePushItem
                                                {
                                                    AbsolutePath = Path.Combine(targetRootFolder,
                                                                                applyChangePushItemRequest.
                                                                                    ChangePushItem.AbsolutePath.Replace(
                                                                                        applyChangePushItemRequest.
                                                                                            SourceRootName, "")),
                                                    OperationType =
                                                        applyChangePushItemRequest.ChangePushItem.OperationType
                                                },
                           Success =
                               _copyStrategy.Copy(applyChangePushItemRequest.ChangePushItem.AbsolutePath,
                                                  Path.Combine(targetRootFolder,
                                                               applyChangePushItemRequest.ChangePushItem.AbsolutePath.
                                                                   Replace(applyChangePushItemRequest.SourceRootName, "")))
                       };
        }

        public virtual TryOpenChannelResponse TryOpenChannel(TryOpenChannelRequest request)
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

        public virtual void Initialize(string targetRootFolder)
        {
            if (string.IsNullOrEmpty(targetRootFolder))
                throw new ArgumentNullException("targetRootFolder");
            _targetRootFolder = targetRootFolder;
        }
        public string MachineName
        {
            get { return Environment.MachineName; }
        }

        public virtual GetHeartbeatResponse GetHeartbeat()
        {
            LoggingManager.Debug("GetHeartbeat will return true.");
            return new GetHeartbeatResponse {Status = true,RootPath=_targetRootFolder};
        }
    }
}
