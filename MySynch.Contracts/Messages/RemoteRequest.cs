using System.ServiceModel;

namespace MySynch.Contracts.Messages
{
    [MessageContract]
    public class RemoteRequest
    {
        [MessageHeader]
        public string FileName { get; set; }
    }
}
