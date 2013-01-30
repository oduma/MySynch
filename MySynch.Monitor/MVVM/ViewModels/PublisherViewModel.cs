using System.Windows.Media;
using MySynch.Contracts.Messages;

namespace MySynch.Monitor.MVVM.ViewModels
{
    internal class PublisherViewModel:ViewModelBase
    {
        public PublisherViewModel(AvailableComponent availablePublisher)
        {
            PublisherName = availablePublisher.Name;
            IsLocal = availablePublisher.IsLocal;
            Status = (availablePublisher.Status==Contracts.Messages.Status.Ok) ? Brushes.Green : Brushes.Red;
        }

        public string PublisherName { get; set; }

        public bool IsLocal { get; set; }

        public SolidColorBrush Status { get; set; }
    }
}
