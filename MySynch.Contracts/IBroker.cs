using System.ServiceModel;
using MySynch.Contracts.Messages;

namespace MySynch.Contracts
{
    [ServiceContract]
    public interface IBroker:ICommunicationComponent
    {

        [OperationContract]
        AttachResponse Attach(AttachRequest request);

        [OperationContract]
        DetachResponse Detach(DetachRequest request);

        [OperationContract(IsOneWay=true)]
        void ReceiveAndDistributeMessage(ReceiveAndDistributeMessageRequest request);

        [OperationContract]
        void MessageReceivedFeedback(MessageReceivedFeedbackRequest request);

    }
}
