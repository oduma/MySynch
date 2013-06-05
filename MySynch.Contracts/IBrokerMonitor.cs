using System.ServiceModel;
using MySynch.Contracts.Messages;

namespace MySynch.Contracts
{
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IBrokerMonitorCallback))]
    public interface IBrokerMonitor:ICommunicationComponent
    {
        [OperationContract(IsOneWay = true)]
        void StartMonitoring();

        [OperationContract]
        ListAllRegistrationsResponse ListAllRegistrations();

        [OperationContract]
        ListAllMessagesResponse ListAllMessages();


    }
}
