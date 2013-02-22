using System.ServiceModel;
using MySynch.Contracts.Messages;

namespace MySynch.Contracts
{
    [ServiceContract]
    public interface IPublisher:ICommunicationComponent
    {
        [OperationContract]
        ChangePushPackage PublishPackage();

        [OperationContract]
        void RemovePackage(ChangePushPackage packagePublished);
    }
}
