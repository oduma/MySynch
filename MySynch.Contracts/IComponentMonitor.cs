using System.ServiceModel;

namespace MySynch.Contracts
{
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IComponentMonitorCallback))]
    public interface IComponentMonitor : ICommunicationComponent
    {
        [OperationContract(IsOneWay = true)]
        void StartMonitoring();

    }
}
