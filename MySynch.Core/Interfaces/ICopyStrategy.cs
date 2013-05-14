using MySynch.Contracts;

namespace MySynch.Core.Interfaces
{
    public interface ICopyStrategy
    {
        bool Copy(string source, string target);

        void Initialize(IPublisher publisher);
    }
}
