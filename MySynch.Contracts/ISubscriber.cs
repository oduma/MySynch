using System.ServiceModel;
using MySynch.Contracts.Messages;

namespace MySynch.Contracts
{
    [ServiceContract]
    public interface ISubscriber:ICommunicationComponent
    {
        [OperationContract]
        ReceiveMessageResponse ReceiveMessage(ReceiveMessageRequest request);

    }
}
