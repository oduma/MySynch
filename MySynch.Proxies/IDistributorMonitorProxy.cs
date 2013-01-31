using MySynch.Contracts;
using MySynch.Core.WCF.Clients;

namespace MySynch.Proxies
{
    public interface IDistributorMonitorProxy : IDistributorMonitor, IInitiateClient
    {
    }
}
