using System.ServiceModel;

namespace MySynch.Contracts.Messages
{
    [MessageContract]
    public class RemoteResponse
    {
        [MessageBodyMember]
        public byte[] Data { get; set; }

    }
}
