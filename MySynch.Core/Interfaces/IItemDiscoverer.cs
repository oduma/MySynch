using System;
using MySynch.Core.DataTypes;

namespace MySynch.Core.Interfaces
{
    public interface IItemDiscoverer
    {
        SynchItem DiscoverFromFolder(string path);

        event EventHandler<FolderDiscoveredArg> DiscoveringFolder;
    }
}
