using System;
using System.Collections.Generic;
using System.IO;
using MySynch.Core.DataTypes;

namespace MySynch.Core
{
    public class FSWatcher
    {
        public string Path { get; private set; }

        private ChangePushPackage _changePackage;
        public FSWatcher(string _folderToWatch,string sourceName)
        {

            FileSystemWatcher fsWatcher = new FileSystemWatcher(_folderToWatch);

            Path = fsWatcher.Path;

            fsWatcher.Created += fsWatcher_Created;
            fsWatcher.Changed += fsWatcher_Changed;
            fsWatcher.Deleted += fsWatcher_Deleted;
            fsWatcher.Renamed += fsWatcher_Renamed;
            fsWatcher.EnableRaisingEvents = true;

            _changePackage = new ChangePushPackage
                                 {
                                     Source = sourceName,
                                     SourceRootName = _folderToWatch,
                                     ChangePushItems = new List<ChangePushItem>()
                                 };

        }

        private void fsWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            //queue a delete
            //queue an insert
            PushADelete(e.OldFullPath);
            PushAnInsert(e.FullPath);

        }

        private void PushAnInsert(string fullPath)
        {
            Console.WriteLine("Pushing an insert for " +fullPath);
        }

        private void PushADelete(string fullPath)
        {
            Console.WriteLine("Pushing an delete for " + fullPath);
        }

        private void fsWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            //queue a delete
            PushADelete(e.FullPath);
        }

        private void fsWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            //queue an update;
            PushAnUpdate(e.FullPath);
        }

        private void PushAnUpdate(string fullPath)
        {
            Console.WriteLine("Pushing an update for " + fullPath);
        }

        private void fsWatcher_Created(object sender, FileSystemEventArgs e)
        {
            //queue an insert;
            PushAnInsert(e.FullPath);
        }

    }
}
