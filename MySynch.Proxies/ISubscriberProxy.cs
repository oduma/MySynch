using MySynch.Contracts;
using MySynch.Core.WCF.Clients;

namespace MySynch.Proxies
{
    public interface ISubscriberProxy:ISubscriber,IInitiateClient
    {
    }
}
