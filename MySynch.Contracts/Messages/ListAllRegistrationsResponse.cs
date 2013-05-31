using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MySynch.Contracts.Messages
{
    [DataContract]
    public class ListAllRegistrationsResponse
    {
        [DataMember]
        public List<Registration> Registrations { get; set; }
    }
}
