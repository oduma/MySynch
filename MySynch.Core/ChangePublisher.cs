using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using MySynch.Common;
using MySynch.Contracts.Messages;
using MySynch.Core.DataTypes;
using MySynch.Core.Interfaces;

namespace MySynch.Core
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
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
            UpdateCurrentRepository(absolutePath, operationType);
            if(ItemsQueued!=null)
                ItemsQueued(this,new ItemsQueuedEventArgs(_temporaryStore));
        }

        private void UpdateCurrentRepository(string absolutePath, OperationType operationType)
        {
            LoggingManager.Debug("Updating CurrentRepository with " + absolutePath);
            switch(operationType)
            {
                case OperationType.Insert:
                        SynchItemManager.AddItem(CurrentRepository, absolutePath, _itemDiscoverer.GetSize(absolutePath));
                    return;
                case OperationType.Update:
                        SynchItemManager.UpdateExistingItem(CurrentRepository, absolutePath, _itemDiscoverer.GetSize(absolutePath));
                    return;
                case OperationType.Delete:
                        SynchItemManager.DeleteItem(CurrentRepository, absolutePath);
                    return;

            }
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

        public void Initialize(string rootFolder,IItemDiscoverer itemDiscoverer)
        {
            if(string.IsNullOrEmpty(rootFolder))
                throw new ArgumentNullException("rootFolder");
            _sourceRootName = rootFolder;
            _itemDiscoverer = itemDiscoverer;
            _temporaryStore = GetOfflineChanges(CurrentRepository);
        }

        internal SortedList<string, OperationType> GetOfflineChanges(SynchItem currentRepository)
        {
            var oldRepository = Serializer.DeserializeFromFile<SynchItem>("backup.xml");
            if (oldRepository == null || oldRepository.Count == 0)
                return new SortedList<string, OperationType>();
            return GetDifferencesBetweenTrees(currentRepository, oldRepository[0]);
        }

        private SortedList<string, OperationType> GetDifferencesBetweenTrees(SynchItem newTree, SynchItem oldTree)
        {
            //flatten both trees
            List<SynchItemData> newTreeFlatten = SynchItemManager.FlattenTree(newTree);
            List<SynchItemData> oldTreeFlatten = SynchItemManager.FlattenTree(oldTree);
            //check if there are any new items in the new tree compared with the oldTree (inserts)
            var itemsToBeInserted = newTreeFlatten.Except(oldTreeFlatten);
            //check if there are any items in the old tree that are not in the newTree (deletions)
            var itemsToBeDeleted = oldTreeFlatten.Except(newTreeFlatten);
            //check if any of the items present in both trees have different size stamp (updates))))
            throw new NotImplementedException();
        }

        public void SaveSettingsEndExit()
        {
            Serializer.SerializeToFile(new List<SynchItem>{CurrentRepository},"backup.xml");
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
                if (PublishedPackageNotDistributed != null && PublishedPackageNotDistributed.Count > 0)
                {
                    LoggingManager.Debug("Republish package: " + PublishedPackageNotDistributed[0].PackageId);
                    return PublishedPackageNotDistributed[0];
                }
                if (_temporaryStore.Keys.Count == 0)
                {
                    LoggingManager.Debug("Nothing to publish");
                    return null;
                }
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

        private SynchItem _currentRepository;
        private IItemDiscoverer _itemDiscoverer;
        internal SynchItem CurrentRepository { get { return _currentRepository = (_currentRepository) ?? _itemDiscoverer.DiscoverFromFolder(RootFolder); } }

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
