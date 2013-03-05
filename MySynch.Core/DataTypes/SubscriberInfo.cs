using System.Xml.Serialization;
using MySynch.Contracts;

namespace MySynch.Core.DataTypes
{
    public class SubscriberInfo:BaseInfo
    {
        [XmlIgnore]
        public ISubscriber Subscriber { get; set; }
    }
}
