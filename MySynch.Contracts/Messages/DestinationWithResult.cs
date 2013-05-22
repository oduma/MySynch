namespace MySynch.Contracts.Messages
{
    public class DestinationWithResult
    {
        public string Url { get; set; }
        public bool Processed { get; set; }
    }
}
