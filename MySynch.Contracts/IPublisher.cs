using System.ServiceModel;
using MySynch.Contracts.Messages;

namespace MySynch.Contracts
{
    [ServiceContract]
    public interface IPublisher:ICommunicationComponent
    {
        [OperationContract]
        PublishPackageRequestResponse PublishPackage();

        [OperationContract]
        void RemovePackage(PublishPackageRequestResponse packageRequestResponsePublished);
    }
}
