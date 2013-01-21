namespace MySynch.Core.DataTypes
{
    public class AvailableChannel
    {
        public PublisherInfo PublisherInfo { get; set; }

        public SubscriberInfo SubscriberInfo { get; set; }

        public string CopyStartegyName { get; set; }

        public string UniqueKey { get; set; }

        public Status Status { get; set; }

        public int NoOfFailedAttempts { get; set; }

    }
}
