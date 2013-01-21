using System;
using MySynch.Core.DataTypes;

namespace MySynch.Core.Interfaces
{
    public interface IChangePublisher:IPublisher
    {
        event EventHandler<ItemsQueuedEventArgs> ItemsQueued;
        
        void QueueInsert(string absolutePath);

        void QueueUpdate(string absolutePath);

        void QueueDelete(string absolutePath);

        void Initialize(string rootFolder);
    }
}
