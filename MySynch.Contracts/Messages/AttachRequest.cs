using System.Runtime.Serialization;

namespace MySynch.Contracts.Messages
{
    [DataContract]
    public class AttachRequest
    {
        [DataMember]
        public Registration RegistrationRequest { get; set; }
    }
}
