using System.Xml.Serialization;
using MySynch.Contracts.Messages;

namespace MySynch.Core.DataTypes
{
    public class AvailableChannel
    {
        public PublisherInfo PublisherInfo { get; set; }

        public SubscriberInfo SubscriberInfo { get; set; }

        [XmlIgnore]
        public Status Status { get; set; }

        [XmlIgnore]
        public int NoOfFailedAttempts { get; set; }

    }
}
