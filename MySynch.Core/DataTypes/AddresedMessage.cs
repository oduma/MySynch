using MySynch.Contracts.Messages;

namespace MySynch.Core.DataTypes
{
    public class AddresedMessage
    {
        public string DestinationUrl { get; set; }

        public bool ProcessedByDestination { get; set; }

        public PublisherMessage OriginalMessage { get; set; }
    }
}
