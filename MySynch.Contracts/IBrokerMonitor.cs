using System.ServiceModel;

namespace MySynch.Contracts
{
    //[ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IDuplexCallback))]
    public interface IBrokerMonitor:ICommunicationComponent
    {
        [OperationContract(IsOneWay = true)]
        void ListAllregistrations();
    }
}
