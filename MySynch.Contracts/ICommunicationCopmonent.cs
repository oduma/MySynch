using System.ServiceModel;
using MySynch.Contracts.Messages;

namespace MySynch.Contracts
{
    [ServiceContract]
    public interface ICommunicationCopmonent
    {
        [OperationContract]
        HeartbeatResponse GetHeartbeat();
    }
}
