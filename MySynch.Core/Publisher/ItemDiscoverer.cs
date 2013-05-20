using System;
using System.Collections.Generic;
using System.IO;
using MySynch.Common.Logging;
using MySynch.Core.DataTypes;

namespace MySynch.Core.Publisher
{
    public static class ItemDiscoverer
    {
        public static SynchItem DiscoverFromFolder(string path)
        {
            LoggingManager.Debug("Discovering from root folder:" +path);
            if(string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");
            if(!Directory.Exists(path))
                throw new ArgumentException("Directory does not exist", "path");
            return new SynchItem
                                             {SynchItemData=new SynchItemData{
                                                 Identifier = path,
                                                 Name =
                                                     path.Substring(path.LastIndexOf(@"\") + 1,
                                                                    path.Length - path.LastIndexOf(@"\") - 1)
                                             },
                                                 Items = GetSubFoldersOrFiles(path)
                                             };
        }

        private static List<SynchItem> GetSubFoldersOrFiles(string path)
        {
            LoggingManager.Debug("Getting sub folders or files "+path);
            List<SynchItem> list= new List<SynchItem>();
            var files = Directory.GetFiles(path);
            if (files.Length != 0)
                foreach (string file in files)
                {
                    list.Add(new SynchItem
                    {
                        SynchItemData = new SynchItemData
                        {
                            Identifier = file,
                            Name =
                                file.Substring(file.LastIndexOf(@"\") + 1,
                                     file.Length - file.LastIndexOf(@"\") - 1),
                            Size = new FileInfo(file).Length
                        }
                    });}
            var folders = Directory.GetDirectories(path);
            if(folders.Length !=0)
                foreach(string folder in folders)
                    list.Add(DiscoverFromFolder(folder));
            LoggingManager.Debug("Got subfolder or file");
            return list;
        }
    }
}
