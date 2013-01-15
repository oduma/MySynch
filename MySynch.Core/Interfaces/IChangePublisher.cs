using MySynch.Core.DataTypes;

namespace MySynch.Core.Interfaces
{
    public interface IChangePublisher
    {
        int MaxItemsPerPackage { get; set; }

        void QueueChange(ChangePushItem item);

        ChangePushPackage PublishPackage();
    }
}
