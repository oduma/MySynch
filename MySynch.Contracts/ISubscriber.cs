using System.ServiceModel;
using MySynch.Contracts.Messages;

namespace MySynch.Contracts
{
    [ServiceContract(CallbackContract=typeof(ISubscriberFeedback))]
    public interface ISubscriber:ICommunicationComponent
    {
        [OperationContract(IsOneWay=true)]
        void ConsumePackage(PublishPackageRequestResponse publishPackageRequestResponse);

        [OperationContract]
        TryOpenChannelResponse TryOpenChannel(TryOpenChannelRequest sourceOfDataEndpointName);

        /// <summary>
        /// For test only
        /// </summary>
        /// <param name="SubscriberFeedback"></param>
        void ForceSetTheSubscriberFeedback(ISubscriberFeedback SubscriberFeedback);
    }
}
