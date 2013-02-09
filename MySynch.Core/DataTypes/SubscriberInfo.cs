﻿using System.Xml.Serialization;
using MySynch.Contracts;

namespace MySynch.Core.DataTypes
{
    public class SubscriberInfo
    {
        [XmlIgnore]
        public IChangeApplyer Subscriber { get; set; }

        public string SubScriberName { get; set; }

        public string EndpointName { get; set; }

        public string TargetRootFolder { get; set; }
    }
}
