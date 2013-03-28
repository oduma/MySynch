using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace MySynch.Contracts.Messages
{
    [DataContract]
    public class MapChannelComponent
    {
        [DataMember]
        public string InstanceName { get; set; }

        [DataMember]
        public int Port { get; set; }

        [DataMember]
        [XmlIgnore]
        public Package CurrentPackage { get; set; }

        [DataMember]
        [XmlIgnore]
        public string RootPath { get; set; }

        [DataMember]
        [XmlIgnore]
        public Status Status { get; set; }
    }
}
