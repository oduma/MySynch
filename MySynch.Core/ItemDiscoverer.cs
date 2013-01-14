using System;
using System.Collections.Generic;
using System.IO;
using MySynch.Core.DataTypes;
using MySynch.Core.Interfaces;

namespace MySynch.Core
{
    public class ItemDiscoverer:IItemDiscoverer
    {
        private string _rootPath;

        private readonly static string _rootPlaceHolder="root";

        public ItemDiscoverer(string rootPath)
        {
            _rootPath = rootPath;
        }

        public SynchItem DiscoverFromFolder(string path)
        {
            if(string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");
            if(!Directory.Exists(path))
                throw new ArgumentException("Directory does not exist", "path");
            if(DiscoveringFolder!=null)
                DiscoveringFolder(this,new FolderDiscoveredArg(path));
            return new SynchItem
                                             {
                                                 Identifier = path.Replace(_rootPath,_rootPlaceHolder),
                                                 Name =
                                                     path.Substring(path.LastIndexOf(@"\") + 1,
                                                                    path.Length - path.LastIndexOf(@"\")-1),
                                                 Items = GetSubFoldersOrFiles(path)
                                             };
        }

        public event EventHandler<FolderDiscoveredArg> DiscoveringFolder;

        private List<SynchItem> GetSubFoldersOrFiles(string path)
        {
            List<SynchItem> list= new List<SynchItem>();
            var files = Directory.GetFiles(path);
            if (files.Length != 0)
                foreach (string file in files)
                    list.Add(new SynchItem
                    {
                        Identifier = file.Replace(_rootPath,_rootPlaceHolder),
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
