﻿using System.Collections.ObjectModel;
using MySynch.Monitor.MVVM.Models;
using MySynch.Monitor.Utils;

namespace MySynch.Monitor.MVVM.ViewModels
{
    internal class NotifyViewModel:ViewModelBase
    {
        public ObservableCollection<NotificationModel> ListAllMessages
        {
            get;
            set;
        }

        public ObservableCollection<RegistrationModel> ListActiveRegistrations
        {
            get;
            set;
        }

        public ObservableCollection<MessageModel> ListActiveMessages { get; set; }
    }
}
