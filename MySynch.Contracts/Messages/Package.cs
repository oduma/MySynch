using System;
using System.Collections.Generic;
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

        [DataMember]
        public List<FeedbackMessage> PackageMessages { get; set; }
    }
}
