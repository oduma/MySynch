using System.ServiceModel;
using MySynch.Contracts.Messages;

namespace MySynch.Contracts
{
    public interface IComponentMonitorCallback
    {
        [OperationContract(IsOneWay = true)]
        void NotifyActivity(PublisherMessage activityMessage);


    }
}
