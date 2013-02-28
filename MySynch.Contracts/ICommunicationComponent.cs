using System.ServiceModel;
using MySynch.Contracts.Messages;

namespace MySynch.Contracts
{
    [ServiceContract]
    public interface ICommunicationComponent
    {
        [OperationContract]
        GetHeartbeatResponse GetHeartbeat();
    }
}
