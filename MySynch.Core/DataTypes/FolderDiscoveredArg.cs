using System;

namespace MySynch.Core.DataTypes
{
    public class FolderDiscoveredArg:EventArgs
    {
        public FolderDiscoveredArg(string path)
        {
            Folder = path;
        }

        public string Folder { get; set; }
    }
}
