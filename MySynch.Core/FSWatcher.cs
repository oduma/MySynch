using System;
using System.IO;
using MySynch.Core.Interfaces;
using MySynch.Core.Utilities;

namespace MySynch.Core
{
    public class FSWatcher
    {
        public string Path { get; private set; }

        private IChangePublisher _changePublisher;

        public FSWatcher(IChangePublisher changePublisher)
        {
            LoggingManager.Debug("Initializing the FS Watcher with publisher: " + changePublisher.RootFolder);
            if(changePublisher==null || string.IsNullOrEmpty(changePublisher.RootFolder))
                throw new ArgumentNullException("changePublisher");
            _changePublisher = changePublisher;

            FileSystemWatcher fsWatcher = new FileSystemWatcher(changePublisher.RootFolder);

            Path = fsWatcher.Path;

            fsWatcher.Created += fsWatcher_Created;
            fsWatcher.Changed += fsWatcher_Changed;
            fsWatcher.Deleted += fsWatcher_Deleted;
            fsWatcher.Renamed += fsWatcher_Renamed;
            fsWatcher.EnableRaisingEvents = true;
            LoggingManager.Debug("Initilization done waiting for changes in the FS.");
        }

        private void fsWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            LoggingManager.Debug("A File renamed from " + e.OldFullPath + " to " + e.FullPath);
            //queue a delete
            //queue an insert
            _changePublisher.QueueDelete(e.OldFullPath);
            _changePublisher.QueueInsert(e.FullPath);

        }

        private void fsWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            LoggingManager.Debug("A file deleted: " + e.FullPath);
            //queue a delete
            _changePublisher.QueueDelete(e.FullPath);
        }

        private void fsWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            LoggingManager.Debug("A file changed: " + e.FullPath);
            //queue an update;
            _changePublisher.QueueUpdate(e.FullPath);
        }

        private void fsWatcher_Created(object sender, FileSystemEventArgs e)
        {
            LoggingManager.Debug("A new file created: " + e.FullPath);
            //queue an insert;
            _changePublisher.QueueInsert(e.FullPath);
        }

    }
}
