using System.ServiceModel;
using MySynch.Contracts.Messages;

namespace MySynch.Contracts
{
    [ServiceContract]
    public interface ISubscriber:ICommunicationComponent
    {
        [OperationContract]
        ApplyChangePackageResponse ApplyChangePackage(PublishPackageRequestResponse publishPackageRequestResponse);

        [OperationContract]
        TryOpenChannelResponse TryOpenChannel(TryOpenChannelRequest sourceOfDataEndpointName);
    }
}
