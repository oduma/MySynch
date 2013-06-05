using System;
using MySynch.Contracts.Messages;

namespace MySynch.Monitor.MVVM.Models
{
    public class MessageModel
    {
        public Guid MessageId { get; set; }
        public string FromUrl { get; set; }
        public OperationType OperationType { get; set; }
        public string ToUrl { get; set; }
        public string File { get; set; }

        public bool Processed { get; set; }
    }
}
