namespace MySynch.Core.WCF.Clients.Discovery
{
    public interface IInitiateClient
    {
        void InitiateUsingPort(int port);

        void DestroyAtPort(int port);
    }
}
