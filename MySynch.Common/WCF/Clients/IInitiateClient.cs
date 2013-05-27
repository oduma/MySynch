using System.ServiceModel;

namespace MySynch.Common.WCF.Clients
{
    public interface IInitiateClient
    {
        void InitiateUsingServerAddress(string serverAddress);
        void InitiateDuplexUsingServerAddress(string serverAddress, InstanceContext callbackInstance);
        void Reset();
    }
}
