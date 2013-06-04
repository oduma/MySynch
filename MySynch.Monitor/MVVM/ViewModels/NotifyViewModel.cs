using System.Collections.ObjectModel;

namespace MySynch.Monitor.MVVM.ViewModels
{
    internal class NotifyViewModel:ViewModelBase
    {
        public ObservableCollection<GenericMessageModel> ListAllMessages
        {
            get;
            set;
        }

        public ObservableCollection<RegistrationModel> ListActiveRegistrations
        {
            get;
            set;
        }

    }
}
