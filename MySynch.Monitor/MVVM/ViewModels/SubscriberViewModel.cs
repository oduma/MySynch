using System.Windows.Media;
using MySynch.Contracts.Messages;
using MySynch.Core.Utilities;

namespace MySynch.Monitor.MVVM.ViewModels
{
    internal class SubscriberViewModel : ViewModelBase
    {
        public SubscriberViewModel(AvailableComponent availableSubscriber)
        {
            using (LoggingManager.LogMySynchPerformance())
            {
                SubscriberName = availableSubscriber.Name;
                IsLocal = availableSubscriber.IsLocal;
                Status = (availableSubscriber.Status == Contracts.Messages.Status.Ok) ? Brushes.Green : Brushes.Red;
            }
        }

        public string SubscriberName { get; set; }

        public bool IsLocal { get; set; }

        public SolidColorBrush Status { get; set; }

    }
}
