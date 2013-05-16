﻿using System.Runtime.Serialization;

namespace MySynch.Contracts.Messages
{
    [DataContract]
    [KnownType(typeof(BrokerMessage))]
    public class PublisherMessage
    {
        [DataMember]
        public string AbsolutePath { get; set; }

        [DataMember]
        public OperationType OperationType { get; set; }

        [DataMember]
        public string SourceOfMessageUrl { get; set; }

        [DataMember]
        public string SourcePathRootName { get; set; }

    }
}