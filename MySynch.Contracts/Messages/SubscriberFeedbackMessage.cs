using System;

namespace MySynch.Contracts.Messages
{
    public class SubscriberFeedbackMessage
    {
        public string TargetAbsolutePath { get; set; }
        public OperationType OperationType { get; set; }
        public bool Success { get; set; }
        public int ItemsProcessed { get; set; }
        public int TotalItemsInThePackage { get; set; }
        public Guid PackageId { get; set; }
    }
}
