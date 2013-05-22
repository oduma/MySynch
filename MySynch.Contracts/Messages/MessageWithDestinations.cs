using System.Collections.Generic;

namespace MySynch.Contracts.Messages
{
    public class MessageWithDestinations:PublisherMessage
    {
        public List<DestinationWithResult> Destinations { get; set; }
    }
}
