using System.Xml.Serialization;
using MySynch.Contracts;

namespace MySynch.Core.DataTypes
{
    public class SubscriberInfo
    {
        [XmlIgnore]
        public ISubscriber Subscriber { get; set; }

        public string SubScriberName { get; set; }

        public int Port { get; set; }
    }
}
