using System;
using MySynch.Contracts;
using MySynch.Contracts.Messages;
using MySynch.Proxies.Interfaces;

namespace MySynch.Tests.Stubs
{
    public class MockRemoteSubscriber:ISubscriberProxy
    {
        private string targetRootFolder;
        private ISubscriberFeedback _subscriberFeedBack;

        public GetHeartbeatResponse GetHeartbeat()
        {
            return new GetHeartbeatResponse {Status = true};
        }

        public void ConsumePackage(PublishPackageRequestResponse publishPackageRequestResponse)
        {
            if (targetRootFolder == "wrong folder")
                throw new Exception();
            bool result = true;

            foreach (ChangePushItem upsert in publishPackageRequestResponse.ChangePushItems)
            {
                var tempResult = copyMethod(upsert.AbsolutePath,
                                             upsert.AbsolutePath.Replace(publishPackageRequestResponse.SourceRootName, targetRootFolder));
                _subscriberFeedBack.SubscriberFeedback(new SubscriberFeedbackMessage
                {
                    ItemsProcessed = 1,
                    OperationType = OperationType.None,
                    PackageId = publishPackageRequestResponse.PackageId,
                    Success = true
                });
            }
        }

        public TryOpenChannelResponse TryOpenChannel(TryOpenChannelRequest sourceOfDataEndpointName)
        {
            return new TryOpenChannelResponse {Status = true};
        }

        public void ForceSetTheSubscriberFeedback(ISubscriberFeedback SubscriberFeedback)
        {
            _subscriberFeedBack = SubscriberFeedback;
        }

        private bool copyMethod(string absolutePath, string replace)
        {
            return true;
        }

        public void InitiateUsingPort(int port)
        {
            return;
        }

        public void DestroyAtPort(int port)
        {
            return;
        }

        public void InitiateDuplexUsingPort<TCallback>(TCallback callbackInstance, int port)
        {
            return;
        }
    }
}
