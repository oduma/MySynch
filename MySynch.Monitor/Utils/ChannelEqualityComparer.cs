using System.Collections.Generic;
using MySynch.Core.DataTypes;

namespace MySynch.Monitor.Utils
{
    internal class ChannelEqualityComparer:IEqualityComparer<AvailableChannel>
    {
        public bool Equals(AvailableChannel x, AvailableChannel y)
        {
            if (x == null || x.PublisherInfo == null || string.IsNullOrEmpty(x.PublisherInfo.InstanceName) || x.SubscriberInfo==null || string.IsNullOrEmpty(x.SubscriberInfo.InstanceName))
                return false;
            if (y == null || y.PublisherInfo == null || string.IsNullOrEmpty(y.PublisherInfo.InstanceName) || y.SubscriberInfo == null || string.IsNullOrEmpty(y.SubscriberInfo.InstanceName))
                return false;
            return (x.PublisherInfo.Port == y.PublisherInfo.Port && x.SubscriberInfo.Port == y.SubscriberInfo.Port);
        }

        public int GetHashCode(AvailableChannel obj)
        {
            return
                (obj.PublisherInfo.InstanceName + obj.PublisherInfo.Port + obj.SubscriberInfo.InstanceName +
                 obj.SubscriberInfo.Port).GetHashCode();
        }
    }
}
