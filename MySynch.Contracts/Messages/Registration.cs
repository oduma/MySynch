using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MySynch.Contracts.Messages
{
    [DataContract]
    public class Registration
    {
        [DataMember]
        public string ServiceUrl { get; set; }

        [DataMember]
        public string MessageMethod { get; set; }

        [DataMember]
        public List<OperationType> OperationTypes { get; set; }

        [DataMember]
        public ServiceRole ServiceRole { get; set; }

    }
}
