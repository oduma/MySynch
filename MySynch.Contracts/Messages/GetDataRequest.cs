using System.ServiceModel;

namespace MySynch.Contracts.Messages
{
    [MessageContract]
    public class GetDataRequest
    {
        [MessageHeader]
        public string FileName { get; set; }
    }
}
