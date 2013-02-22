using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MySynch.Contracts.Messages
{
    [DataContract]
    public class SynchItem
    {
        [DataMember]
        public string Name
        {
            get;
            set;
        }

        [DataMember]
        public string Identifier
        {
            get;
            set;
        }

        [DataMember]
        public bool Changed { get; set; }

        [DataMember]
        public List<SynchItem> Items
        {
            get;
            set;
        }
    }
}
