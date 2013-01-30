using System.Windows.Media;
using MySynch.Contracts.Messages;

namespace MySynch.Monitor.MVVM.ViewModels
{
    internal class SubscriberViewModel : ViewModelBase
    {
        public SubscriberViewModel(AvailableComponent availableSubscriber)
        {
            SubscriberName = availableSubscriber.Name;
            IsLocal = availableSubscriber.IsLocal;
            Status = (availableSubscriber.Status == Contracts.Messages.Status.Ok) ? Brushes.Green : Brushes.Red;
        }

        public string SubscriberName { get; set; }

        public bool IsLocal { get; set; }

        public SolidColorBrush Status { get; set; }

    }
}
