using System;
using MySynch.Core.DataTypes;

namespace MySynch.Core.Interfaces
{
    public interface IChangePublisher
    {
        event EventHandler<ItemsQueuedEventArgs> ItemsQueued;
        
        void QueueInsert(string absolutePath);

        void QueueUpdate(string absolutePath);

        void QueueDelete(string absolutePath);

        ChangePushPackage PublishPackage();
    }
}
