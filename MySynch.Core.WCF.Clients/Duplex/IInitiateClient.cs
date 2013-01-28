namespace MySynch.Core.WCF.Clients.Duplex
{
    public interface IInitiateClient<TCallback>
    {
        void InitiateUsingEndpoint(TCallback callbackInstance, string endpointName);
    }
}
