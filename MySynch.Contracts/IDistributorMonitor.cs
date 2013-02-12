using System.ServiceModel;
using MySynch.Contracts.Messages;

namespace MySynch.Contracts
{
    [ServiceContract]
    public interface IDistributorMonitor:ICommunicationComponent
    {
        [OperationContract]
        DistributorComponent ListAvailableComponentsTree();
    }
}
