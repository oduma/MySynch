using System.Collections.Generic;
using MySynch.Contracts.Messages;
using MySynch.Core.DataTypes;

namespace MySynch.Core.Interfaces
{
    public interface IChangeDiscoverer
    {
        void DiscoverChanges(List<SynchItem> sourceChanges, string targetPool);
    }
}
