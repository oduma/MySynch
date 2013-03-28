namespace MySynch.Core.WCF.Clients.Discovery
{
    public interface IInitiateClient
    {
        void InitiateUsingPort(int port);

        void DestroyAtPort(int port);

        void InitiateDuplexUsingPort<TCallback>(TCallback callbackInstance, int port);

    }
}
