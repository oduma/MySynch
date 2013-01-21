using System;
using System.IO;
using MySynch.Core.DataTypes;
using MySynch.Core.Interfaces;

namespace MySynch.Core
{
    public class FSWatcher
    {
        public string Path { get; private set; }

        public event EventHandler<ItemsQueuedEventArgs> ItemsQueued;

        private IChangePublisher _changePublisher;

        public FSWatcher(string _folderToWatch)
        {
            _changePublisher = new ChangePublisher();
            _changePublisher.Initialize(_folderToWatch);
            _changePublisher.ItemsQueued += _changePublisher_ItemsQueued;

            FileSystemWatcher fsWatcher = new FileSystemWatcher(_folderToWatch);

            Path = fsWatcher.Path;

            fsWatcher.Created += fsWatcher_Created;
            fsWatcher.Changed += fsWatcher_Changed;
            fsWatcher.Deleted += fsWatcher_Deleted;
            fsWatcher.Renamed += fsWatcher_Renamed;
            fsWatcher.EnableRaisingEvents = true;
        }

        void _changePublisher_ItemsQueued(object sender, ItemsQueuedEventArgs e)
        {
            if (ItemsQueued != null)
                ItemsQueued(this, e);
        }

        private void fsWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            //queue a delete
            //queue an insert
            _changePublisher.QueueDelete(e.OldFullPath);
            _changePublisher.QueueInsert(e.FullPath);
        }

        private void fsWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            //queue a delete
            _changePublisher.QueueDelete(e.FullPath);
        }

        private void fsWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            //queue an update;
            _changePublisher.QueueUpdate(e.FullPath);
        }

        private void fsWatcher_Created(object sender, FileSystemEventArgs e)
        {
            //queue an insert;
            _changePublisher.QueueInsert(e.FullPath);
        }

    }
}
