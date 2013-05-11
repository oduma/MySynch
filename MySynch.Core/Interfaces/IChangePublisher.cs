using MySynch.Contracts;
using MySynch.Core.Publisher;

namespace MySynch.Core.Interfaces
{
    public interface IChangePublisher:IPublisher
    {
        void QueueInsert(string absolutePath);

        void QueueUpdate(string absolutePath);

        void QueueDelete(string absolutePath);

        void Initialize(string rootFolder,ItemDiscoverer itemDiscoverer);

        void SaveSettingsEndExit();

        string RootFolder { get; }
    }
}
