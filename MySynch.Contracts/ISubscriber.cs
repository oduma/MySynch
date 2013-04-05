using System.ServiceModel;
using MySynch.Contracts.Messages;

namespace MySynch.Contracts
{
    [ServiceContract]
    public interface ISubscriber:ICommunicationComponent
    {
        [OperationContract]
        ApplyChangePushItemResponse ApplyChangePushItem(ApplyChangePushItemRequest applyChangePushItemRequest);

        [OperationContract]
        TryOpenChannelResponse TryOpenChannel(TryOpenChannelRequest sourceOfDataEndpointName);

    }
}
