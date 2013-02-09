using System.ServiceModel;

namespace MySynch.Contracts.Messages
{
    [MessageContract]
    public class RemoteResponse
    {
        [MessageBodyMember]
        private byte[] Data { get; set; }
    }
}
