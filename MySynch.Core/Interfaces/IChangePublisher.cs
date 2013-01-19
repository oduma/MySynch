using System.Collections.Generic;
using MySynch.Core.DataTypes;

namespace MySynch.Core.Interfaces
{
    public interface IChangePublisher
    {
        void QueueInsert(string absolutePath);

        void QueueUpdate(string absolutePath);

        void QueueDelete(string absolutePath);

        ChangePushPackage PublishPackage();
    }
}
