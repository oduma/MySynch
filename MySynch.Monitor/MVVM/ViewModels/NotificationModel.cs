using System;

namespace MySynch.Monitor.MVVM.ViewModels
{
    internal class NotificationModel
    {
        public string Message { get; set; }

        public ComponentType Source { get; set; }

        public DateTime DateOfEvent { get; set; }
    }
}
