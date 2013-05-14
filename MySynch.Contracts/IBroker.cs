using System.ServiceModel;
using MySynch.Contracts.Messages;

namespace MySynch.Contracts
{
    [ServiceContract]
    //[ApplyCustomClientBehavior(BehaviorType="Publisher")]
    public interface IBroker:ICommunicationComponent
    {

        [OperationContract]
        AttachResponse Attach(AttachRequest request);

        [OperationContract]
        DetachResponse Detach(DetachRequest request);

        [OperationContract]
        ListAllRegistrationsResponse ListAllRegistrations();

        [OperationContract(IsOneWay=true)]
        void ReceiveAndDistributeMessage(ReceiveAndDistributeMessageRequest request);

        [OperationContract]
        ListAllMessagesResponse ListAllMessages();

    }
}
