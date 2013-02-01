using System;
using System.Collections.Generic;
using System.Linq;
using MySynch.Common;
using MySynch.Contracts.Messages;
using MySynch.Core.DataTypes;
using MySynch.Core.Interfaces;

namespace MySynch.Core
{
    public class ChangePublisher: IChangePublisher
    {
        private SortedList<string,OperationType> _temporaryStore;
        private object _lock= new object();
        private string _sourceRootName;

        public ChangePublisher()
        {
            LoggingManager.Debug("");
            _temporaryStore=new SortedList<string, OperationType>();
        }

        public event EventHandler<ItemsQueuedEventArgs> ItemsQueued;

        public void QueueInsert(string absolutePath)
        {
            if (string.IsNullOrEmpty(absolutePath))
                return;
            lock (_lock)
            {
                LoggingManager.Debug("Will queue an insert for: " + absolutePath);
                WriteIfNew(absolutePath, OperationType.Insert);
            }
        }

        private void WriteIfNew(string absolutePath, OperationType operationType)
        {
            LoggingManager.Debug("Adding to the temporary store of the queue");
            if (_temporaryStore.ContainsKey(absolutePath))
                _temporaryStore[absolutePath] = operationType;
            else
                _temporaryStore.Add(absolutePath, operationType);
            if(ItemsQueued!=null)
                ItemsQueued(this,new ItemsQueuedEventArgs(_temporaryStore));
        }

        public void QueueUpdate(string absolutePath)
        {
            if (string.IsNullOrEmpty(absolutePath))
                return;
            lock (_lock)
            {
                LoggingManager.Debug("Will queue an update for: " + absolutePath);
                WriteIfNew(absolutePath, OperationType.Update);
            }
        }

        public void QueueDelete(string absolutePath)
        {
            if (string.IsNullOrEmpty(absolutePath))
                return;
            lock (_lock)
            {
                LoggingManager.Debug("Will queue a delete for: " + absolutePath);
                WriteIfNew(absolutePath, OperationType.Delete);
            }
        }

        public void Initialize(string rootFolder)
        {
            if(string.IsNullOrEmpty(rootFolder))
                throw new ArgumentNullException("rootFolder");
            _sourceRootName = rootFolder;
        }

        public string RootFolder
        {
            get { return _sourceRootName; }
        }

        public ChangePushPackage PublishPackage()
        {
            if(string.IsNullOrEmpty(_sourceRootName) || _temporaryStore==null)
                throw new PublisherSetupException(_sourceRootName,"Publisher Information incomplete");
            lock (_lock)
            {
                LoggingManager.Debug("Starting publishing package");
                ChangePushPackage changePushPackage = new ChangePushPackage
                                                          {Source = Environment.MachineName,
                                                              SourceRootName = _sourceRootName,PackageId=Guid.NewGuid()};
                List<ChangePushItem> _pushItems= new List<ChangePushItem>();
                foreach(string key in _temporaryStore.Keys)
                    _pushItems.Add(new ChangePushItem{AbsolutePath = key,OperationType = _temporaryStore[key]});
                _temporaryStore.Clear();
                changePushPackage.ChangePushItems = _pushItems;
                if(PublishedPackageNotDistributed==null)
                    PublishedPackageNotDistributed= new List<ChangePushPackage>();
                PublishedPackageNotDistributed.Add(changePushPackage);
                LoggingManager.Debug("Published package: " +changePushPackage.PackageId);
                return changePushPackage;
            }
        }

        public void RemovePackage(ChangePushPackage packagePublished)
        {
            if(packagePublished==null)
                throw new ArgumentNullException("publishedPackage");
            lock (_lock)
            {
                LoggingManager.Debug("Removing package after publishing: " + packagePublished.PackageId);
                var identifiedPackage =
                    PublishedPackageNotDistributed.FirstOrDefault(p => p.PackageId == packagePublished.PackageId);
                if (identifiedPackage == null)
                    return;
                PublishedPackageNotDistributed.Remove(identifiedPackage);
                LoggingManager.Debug("Removed package after publishing: " + packagePublished.PackageId);
            }
        }

        public string MachineName
        {
            get { return Environment.MachineName; }
        }

        public HeartbeatResponse GetHeartbeat()
        {
            LoggingManager.Debug("GetHeartbeat will return true.");
            return new HeartbeatResponse {Status = true};
        }

        internal List<ChangePushPackage> PublishedPackageNotDistributed { get; set; }
    }
}
