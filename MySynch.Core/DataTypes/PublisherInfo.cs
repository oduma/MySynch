using MySynch.Contracts;

namespace MySynch.Core.DataTypes
{
    public class PublisherInfo
    {

        public IPublisher Publisher { get; set; }

        public string PublisherInstanceName { get; set; }

        public string EndpointName { get; set; }
    }
}
