using System.ServiceModel;
using MySynch.Contracts.Messages;

namespace MySynch.Contracts
{

    public interface IBrokerMonitorCallback
    {
        [OperationContract(IsOneWay=true)]
        void NotifyRegistrationChange(Registration changedRegistration);

        [OperationContract(IsOneWay = true)]
        void NotifyMessageFlow(MessageWithDestinations messageWithDestinations);
    }
}
