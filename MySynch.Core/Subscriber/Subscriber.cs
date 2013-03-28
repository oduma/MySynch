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
        internal ISubscriberFeedback SubscriberFeedback { get; set; }

        internal ICopyStrategy CopyStrategy
        {
            set { _copyStrategy = value; }
        }

        public virtual void ConsumePackage(PublishPackageRequestResponse publishPackageRequestResponse)
        {
            LoggingManager.Debug("Trying to apply some changes to: " + _targetRootFolder);
            if (OperationContext.Current != null)
            {
                LoggingManager.Debug("In a service so setting the feedback to caller");
                SubscriberFeedback = OperationContext.Current.GetCallbackChannel<ISubscriberFeedback>();
            }
            else
                LoggingManager.Debug("Not in a service so the feedback should be forceibly");

            if (publishPackageRequestResponse == null || publishPackageRequestResponse.ChangePushItems == null || publishPackageRequestResponse.ChangePushItems.Count <= 0)
            {
                LoggingManager.Debug("Nothing to apply.");
                return;
            }
            if (_copyStrategy == null)
            {
                throw new SourceOfDataSetupException("", "No source of data established");
            }
            TryApplyChanges(publishPackageRequestResponse);
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

        public virtual void ForceSetTheSubscriberFeedback(ISubscriberFeedback SubscriberFeedback)
        {
            this.SubscriberFeedback = SubscriberFeedback;
        }

        internal bool TryApplyChanges(PublishPackageRequestResponse publishPackageRequestResponse)
        {
            var upserts =
                publishPackageRequestResponse.ChangePushItems.Where(
                    i => i.OperationType == OperationType.Insert || i.OperationType == OperationType.Update);
            var response = ApplyUpserts(upserts,
                                      _targetRootFolder, publishPackageRequestResponse.SourceRootName, publishPackageRequestResponse.ChangePushItems.Count,publishPackageRequestResponse.PackageId) &&
                         ApplyDeletes(publishPackageRequestResponse.ChangePushItems.Where(i => i.OperationType == OperationType.Delete),
                                      _targetRootFolder, publishPackageRequestResponse.SourceRootName, publishPackageRequestResponse.ChangePushItems.Count,upserts.Count(),publishPackageRequestResponse.PackageId);
            LoggingManager.Debug("Result of applying changes is: " +response);
            return response;
        }

        public virtual void Initialize(string targetRootFolder)
        {
            if (string.IsNullOrEmpty(targetRootFolder))
                throw new ArgumentNullException("targetRootFolder");
            _targetRootFolder = targetRootFolder;
        }

        private bool ApplyDeletes(IEnumerable<ChangePushItem> deletes, string targetRootFolder, string sourceRootName, int totalItemsToProcess, int itemsPreviouslyProcessed, Guid packageId)
        {
            LoggingManager.Debug("Applying deletes to " + targetRootFolder);

            bool result = true;

            foreach (ChangePushItem delete in deletes)
            {
                delete.AbsolutePath = delete.AbsolutePath.Replace(sourceRootName, targetRootFolder);

                var subscriberFeedbackMessage = new SubscriberFeedbackMessage
                                                    {
                                                        TargetAbsolutePath = delete.AbsolutePath,
                                                        OperationType=OperationType.Delete,
                                                        ItemsProcessed = itemsPreviouslyProcessed,
                                                        TotalItemsInThePackage = totalItemsToProcess,
                                                        PackageId=packageId
                                                    };
                if (File.Exists(delete.AbsolutePath))
                {
                    if(SubscriberFeedback!=null)
                        SubscriberFeedback.SubscriberFeedback( subscriberFeedbackMessage);
                    File.Delete(delete.AbsolutePath);
                    subscriberFeedbackMessage.Success = true;
                    subscriberFeedbackMessage.ItemsProcessed = itemsPreviouslyProcessed++;
                    if (SubscriberFeedback != null)
                        SubscriberFeedback.SubscriberFeedback(subscriberFeedbackMessage);
                }
                else
                {
                    if(SubscriberFeedback!=null)
                        SubscriberFeedback.SubscriberFeedback(subscriberFeedbackMessage);
                    result = false;
                    subscriberFeedbackMessage.ItemsProcessed = itemsPreviouslyProcessed++;
                    if (SubscriberFeedback != null)
                        SubscriberFeedback.SubscriberFeedback(subscriberFeedbackMessage);
                }
            }
            LoggingManager.Debug("Apply deletes returns " + result);

            return result;
        }

        private bool ApplyUpserts(IEnumerable<ChangePushItem> upserts, string targetRootFolder, string sourceRootName,int totalItemsToProcess, Guid packageId)
        {
            LoggingManager.Debug("Applying upserts from " + sourceRootName + " to " +targetRootFolder);
            bool result = true;
            var itemsProcessed = 0;
            foreach (ChangePushItem upsert in upserts)
            {
                var subscriberFeedbackMessage = new SubscriberFeedbackMessage
                                                    {
                                                        ItemsProcessed = itemsProcessed,
                                                        OperationType = upsert.OperationType,
                                                        TargetAbsolutePath =
                                                            Path.Combine(targetRootFolder,
                                                                         upsert.AbsolutePath.Replace(sourceRootName, "")),
                                                        TotalItemsInThePackage = totalItemsToProcess,
                                                        PackageId=packageId
                                                    };
                if (SubscriberFeedback != null)
                    SubscriberFeedback.SubscriberFeedback(subscriberFeedbackMessage);
                var tempResult = _copyStrategy.Copy(upsert.AbsolutePath,subscriberFeedbackMessage.TargetAbsolutePath);
                subscriberFeedbackMessage.Success = tempResult;
                subscriberFeedbackMessage.ItemsProcessed = itemsProcessed++;
                if (SubscriberFeedback != null)
                    SubscriberFeedback.SubscriberFeedback(subscriberFeedbackMessage);
                result = result && tempResult;
            }
            LoggingManager.Debug("Apply upserts returns "+ result);
            return result;
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
