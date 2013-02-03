using System;
using System.Runtime.Serialization;

namespace MySynch.Contracts.Messages
{
    [DataContract]
    public class Package
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public State State { get; set; }
    }
}
