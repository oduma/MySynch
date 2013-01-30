using System.ServiceModel;
using MySynch.Contracts.Messages;

namespace MySynch.Contracts
{
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IDistributorCallbacks))]
    public interface IDistributorMonitor:ICommunicationCopmonent
    {
        [OperationContract]
        DistributorComponent ListAvailableComponentsTree();
    }
}
