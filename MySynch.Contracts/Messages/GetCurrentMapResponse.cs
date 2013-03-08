using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MySynch.Contracts.Messages
{
    [DataContract]
    public class GetCurrentMapResponse
    {
        [DataMember]
        public List<MapChannel> MapChannels { get; set; }
    }
}
