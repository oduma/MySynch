using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MySynch.Core.DataTypes;

namespace MySynch.Core
{
    public static class ItemDiscoverer
    {
        public static SynchItem DiscoverFromFolder(string path)
        {
            if(string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");
            if(!Directory.Exists(path))
                throw new ArgumentException("Directory does not exist", "path");
            return new SynchItem
                                             {
                                                 Identifier = path,
                                                 Name =
                                                     path.Substring(path.LastIndexOf(@"\") + 1,
                                                                    path.Length - path.LastIndexOf(@"\")-1),
                                                 Items = GetSubFoldersOrFiles(path)
                                             };
        }

        private static List<SynchItem> GetSubFoldersOrFiles(string path)
        {
            List<SynchItem> list= new List<SynchItem>();
            var files = Directory.GetFiles(path);
            if (files.Length != 0)
                foreach (string file in files)
                    list.Add(new SynchItem
                    {
                        Identifier = file,
                        Name =
                            file.Substring(file.LastIndexOf(@"\") + 1,
                                 file.Length - file.LastIndexOf(@"\") - 1)
                    });
            var folders = Directory.GetDirectories(path);
            if(folders.Length !=0)
                foreach(string folder in folders)
                    list.Add(DiscoverFromFolder(folder));
            return list;
        }
    }
}
