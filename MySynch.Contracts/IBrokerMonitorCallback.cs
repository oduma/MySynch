using System.ServiceModel;

namespace MySynch.Contracts
{

    public interface IBrokerMonitorCallback
    {
        [OperationContract(IsOneWay=true)]
        void ListAllRegistrationsCallback(string somethingback);
    }
}
