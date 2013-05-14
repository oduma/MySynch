using System.ServiceModel;

namespace MySynch.Contracts.Messages
{
    [MessageContract]
    public class GetDataResponse
    {
        [MessageBodyMember]
        public byte[] Data { get; set; }

    }
}
