using System.Xml.Serialization;
using MySynch.Contracts;
using MySynch.Core.Interfaces;

namespace MySynch.Core.DataTypes
{
    public class DataSourceInfo
    {
        [XmlIgnore]
        public ISourceOfData DataSource { get; set; }

        public string DataSourceName { get; set; }

        public string EndpointName { get; set; }

        public string CopyStartegyName { get; set; }

        [XmlIgnore]
        public ICopyStrategy CopyStrategy { get; set; }


    }
}
