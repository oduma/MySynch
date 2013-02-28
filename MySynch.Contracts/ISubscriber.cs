using System.Collections.Generic;
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
        GetTargetFolderResponse GetTargetRootFolder();

        [OperationContract]
        TryOpenChannelResponse TryOpenChannel(TryOpenChannelRequest sourceOfDataEndpointName);
    }
}
