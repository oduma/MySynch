using MySynch.Contracts;
using MySynch.Core.WCF.Clients;

namespace MySynch.Proxies
{
    public interface ISourceOfDataProxy:ISourceOfData,IInitiateClient
    {
    }
}
