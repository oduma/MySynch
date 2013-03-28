using System;
using MySynch.Contracts;
using MySynch.Contracts.Messages;

namespace MySynch.Tests.Stubs
{
    public class FakeSubscriber:ISubscriber
    {
        private ISubscriberFeedback _subscriberFeedBack;

        public GetHeartbeatResponse GetHeartbeat()
        {
            return new GetHeartbeatResponse {Status = true, RootPath = @"destination root folder\Item One"};
        }

        public void ConsumePackage(PublishPackageRequestResponse publishPackageRequestResponse)
        {
            _subscriberFeedBack.SubscriberFeedback(new SubscriberFeedbackMessage
                                                       {
                                                           ItemsProcessed = 1,
                                                           OperationType = OperationType.None,
                                                           PackageId = publishPackageRequestResponse.PackageId,
                                                           Success = true
                                                       });
        }

        public TryOpenChannelResponse TryOpenChannel(TryOpenChannelRequest sourceOfDataEndpointName)
        {
            return new TryOpenChannelResponse {Status = true};
        }

        public void ForceSetTheSubscriberFeedback(ISubscriberFeedback SubscriberFeedback)
        {
            _subscriberFeedBack = SubscriberFeedback;
        }
    }
}
