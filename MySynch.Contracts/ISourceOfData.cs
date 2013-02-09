using System.ServiceModel;
using MySynch.Contracts.Messages;

namespace MySynch.Contracts
{
    [ServiceContract]
    public interface ISourceOfData
    {
        [OperationContract]
        RemoteResponse GetData(RemoteRequest remoteRequest);
    }
}
