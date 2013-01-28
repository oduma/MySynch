using System.ServiceModel;

namespace MySynch.Contracts
{
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IDistributorCallbacks))]
    public interface IDistributorMonitor
    {

    }
}
