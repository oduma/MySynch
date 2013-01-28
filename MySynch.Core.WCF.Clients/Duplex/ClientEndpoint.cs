namespace MySynch.Core.WCF.Clients.Duplex
{
    internal class ClientEndpoint<T>
    {

        public ClientEndpoint(string endpointName)
        {
            EndpointIdentifier = typeof(T).Name + endpointName;
        }

        public EndpointChannelFactory<T> Endpoint { get; set; }

        public string EndpointIdentifier { get; set; }
    }
}
