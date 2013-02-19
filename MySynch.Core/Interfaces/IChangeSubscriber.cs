using MySynch.Contracts;

namespace MySynch.Core.Interfaces
{
    public interface IChangeSubscriber:ISubscriber
    {
        void Initialize(string targetRootFolder);
    }
}
