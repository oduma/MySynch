using MySynch.Contracts;
using MySynch.Core.WCF.Clients.Duplex;

namespace MySynch.Proxies
{
    public interface IDistributorMonitorProxy : IDistributorMonitor, IInitiateClient<IDistributorCallbacks>
    {
    }
}
