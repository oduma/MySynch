using System.ServiceModel;
using MySynch.Contracts.Messages;

namespace MySynch.Contracts
{
    [ServiceContract]
    public interface IPublisher:ICommunicationCopmonent
    {
        [OperationContract]
        ChangePushPackage PublishPackage();
    }
}
