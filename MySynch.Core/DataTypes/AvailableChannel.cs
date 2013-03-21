using System.Xml.Serialization;
using MySynch.Contracts;
using MySynch.Contracts.Messages;

namespace MySynch.Core.DataTypes
{
    public class AvailableChannel:MapChannel
    {
        [XmlIgnore]
        public IPublisher Publisher { get; set; }

        [XmlIgnore]
        public ISubscriber Subscriber { get; set; }

        [XmlIgnore]
        public int NoOfFailedAttempts { get; set; }

    }
}
