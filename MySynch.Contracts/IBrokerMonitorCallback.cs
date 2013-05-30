using System.ServiceModel;
using MySynch.Contracts.Messages;

namespace MySynch.Contracts
{

    public interface IBrokerMonitorCallback
    {
        [OperationContract(IsOneWay=true)]
        void NotifyNewRegistration(Registration changedRegistration);

        [OperationContract(IsOneWay = true)]
        void NotifyRemoveRegistration(Registration changedRegistration);

        [OperationContract(IsOneWay = true)]
        void NotifyNewMessage(MessageWithDestinations msg);

        [OperationContract(IsOneWay = true)]
        void NotifyMessageUpdate(MessageWithDestinations msg);

        [OperationContract(IsOneWay = true)]
        void NotifyMessageDelete(MessageWithDestinations deletedMessage);
    }
}
