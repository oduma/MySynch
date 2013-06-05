using System.Collections.Generic;
using System.ServiceModel;
using MySynch.Contracts.Messages;

namespace MySynch.Contracts
{

    public interface IBrokerMonitorCallback
    {
        [OperationContract(IsOneWay=true)]
        void NotifyNewRegistration(Registration changedRegistration, List<Registration> registrations);

        [OperationContract(IsOneWay = true)]
        void NotifyRemoveRegistration(Registration changedRegistration, List<Registration> registrations);

        [OperationContract(IsOneWay = true)]
        void NotifyNewMessage(MessageWithDestinations msg, List<MessageWithDestinations> messages);

        [OperationContract(IsOneWay = true)]
        void NotifyMessageUpdate(MessageWithDestinations msg, List<MessageWithDestinations> messages);

        [OperationContract(IsOneWay = true)]
        void NotifyMessageDelete(MessageWithDestinations deletedMessage, List<MessageWithDestinations> messages);
    }
}
