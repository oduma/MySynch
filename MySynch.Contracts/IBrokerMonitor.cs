using System.ServiceModel;

namespace MySynch.Contracts
{
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IBrokerMonitorCallback))]
    public interface IBrokerMonitor:ICommunicationComponent
    {
        [OperationContract(IsOneWay = true)]
        void ListAllregistrationsForDuplex();
    }
}
