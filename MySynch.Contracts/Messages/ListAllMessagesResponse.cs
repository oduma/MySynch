﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MySynch.Contracts.Messages
{
    [DataContract]
    public class ListAllMessagesResponse
    {
        [DataMember]
        public List<BrokerMessage> AvailableMessages { get; set; }
    }
}