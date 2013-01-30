using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MySynch.Contracts.Messages
{
    [DataContract]
    public class DistributorComponent
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public List<AvailableComponent> AvailablePublishers { get; set; }
    }
}
