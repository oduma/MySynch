using System;
using MySynch.Monitor.MVVM.ViewModels;

namespace MySynch.Monitor.MVVM.Models
{
    internal class NotificationModel
    {
        public string Message { get; set; }

        public ComponentType Source { get; set; }

        public DateTime DateOfEvent { get; set; }
    }
}
