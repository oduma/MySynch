using System.Collections.ObjectModel;
using System.Windows;
using MySynch.Monitor.MVVM.Models;

namespace MySynch.Monitor.MVVM.ViewModels
{
    internal class NotifyViewModel:ViewModelBase
    {
        private Visibility _brokerVisible = Visibility.Visible;
        public Visibility BrokerVisible
        {
            get
            {
                return this._brokerVisible;
            }

            set
            {
                if (value != _brokerVisible)
                {
                    _brokerVisible = value;
                    RaisePropertyChanged("BrokerVisible");
                }
            }
        }

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
