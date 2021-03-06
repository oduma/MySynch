﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MySynch.Contracts.Messages
{
    [DataContract]
    public class ListAvailableChannelsResponse
    {

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public List<MapChannel> Channels { get; set; }
    }
}
