using System.Xml.Serialization;
using MySynch.Contracts;

namespace MySynch.Core.DataTypes
{
    public class PublisherInfo:BaseInfo
    {
        [XmlIgnore]
        public IPublisher Publisher { get; set; }
    }
}
