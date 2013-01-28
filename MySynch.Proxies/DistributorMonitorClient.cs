using MySynch.Contracts;
using MySynch.Core.WCF.Clients.Duplex;

namespace MySynch.Proxies
{
    public class DistributorMonitorClient:BaseClient<IDistributorMonitor,IDistributorCallbacks>, IDistributorMonitorProxy
    {
    }
}
