using MySynch.Contracts;
using MySynch.Core.WCF.Clients;

namespace MySynch.Proxies
{
    public interface IPublisherProxy:IPublisher,IInitiateClient
    {
    }
}
