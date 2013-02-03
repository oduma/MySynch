using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MySynch.Contracts.Messages
{
    [DataContract]
    public class AvailableComponent
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool IsLocal { get; set; }

        [DataMember]
        public Status Status { get; set; }

        [DataMember]
        public List<AvailableComponent> DependentComponents { get; set; }

        [DataMember]
        public List<Package> Packages { get; set; }
    }
}
