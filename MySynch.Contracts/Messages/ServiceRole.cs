using System.Runtime.Serialization;

namespace MySynch.Contracts.Messages
{
    [DataContract]
    public enum ServiceRole
    {
        [EnumMember]
        Publisher=1,
        [EnumMember]
        Subscriber=2
    }
}
