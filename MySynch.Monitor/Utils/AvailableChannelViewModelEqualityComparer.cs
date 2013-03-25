using System.Collections.Generic;
using MySynch.Monitor.MVVM.ViewModels;

namespace MySynch.Monitor.Utils
{
    class AvailableChannelViewModelEqualityComparer:IEqualityComparer<AvailableChannelViewModel>
    {
        public bool Equals(AvailableChannelViewModel x, AvailableChannelViewModel y)
        {
            if (x == null || y == null)
                return false;
            if (string.IsNullOrEmpty(x.MapChannelPublisherTitle) 
                || string.IsNullOrEmpty(x.MapChannelSubscriberTitle) 
                || string.IsNullOrEmpty(y.MapChannelPublisherTitle) 
                || string.IsNullOrEmpty(y.MapChannelSubscriberTitle))
                return false;
            return (x.MapChannelPublisherTitle == y.MapChannelPublisherTitle &&
                    x.MapChannelSubscriberTitle == y.MapChannelSubscriberTitle);
        }

        public int GetHashCode(AvailableChannelViewModel obj)
        {
            return obj.GetHashCode();
        }
    }
}
