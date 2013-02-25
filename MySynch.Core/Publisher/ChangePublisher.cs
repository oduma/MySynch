using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using MySynch.Common;
using MySynch.Contracts.Messages;
using MySynch.Core.DataTypes;
using MySynch.Core.Interfaces;

namespace MySynch.Core.Publisher
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
            try
            {
                if (string.IsNullOrEmpty(rootFolder))
                    throw new ArgumentNullException("rootFolder");
                _sourceRootName = rootFolder;
                _itemDiscoverer = itemDiscoverer;
                _temporaryStore = GetOfflineChanges(CurrentRepository);

            }
            catch (Exception ex)
            {
                LoggingManager.LogMySynchSystemError(ex);
            }
        }

        internal SortedList<string, OperationType> GetOfflineChanges(SynchItem currentRepository)
        {
            if(currentRepository==null)
                throw new ArgumentNullException("currentRepository");
            var oldRepository = Serializer.DeserializeFromFile<SynchItem>("backup.xml");
            if(oldRepository.Count==0)
                return new SortedList<string, OperationType>();
            return GetDifferencesBetweenTrees(currentRepository, (oldRepository==null || oldRepository.Count==0)?new SynchItem(): oldRepository[0]);
        }

        internal static SortedList<string, OperationType> GetDifferencesBetweenTrees(SynchItem newTree, SynchItem oldTree)
        {
            List<SynchItemData> newTreeFlatten = SynchItemManager.FlattenTree(newTree);
            List<SynchItemData> oldTreeFlatten = SynchItemManager.FlattenTree(oldTree);
            SortedList<string,OperationType> result = new SortedList<string, OperationType>();
            if(newTree.Items==null)
            {
                if (oldTree.Items == null)
                    return new SortedList<string, OperationType>();
                else
                {
                    foreach (SynchItemData synchItemData in oldTreeFlatten)
                        result.Add(synchItemData.Identifier, OperationType.Delete);
                    return result;
                }
            }
            else
            {
                if (oldTree.Items == null)
                {
                    foreach(SynchItemData synchItemData in newTreeFlatten)
                        result.Add(synchItemData.Identifier,OperationType.Insert);
                    return result;
                }
            }
            if(newTree.SynchItemData.Identifier!=oldTree.SynchItemData.Identifier)
                throw new PublisherSetupException(newTree.SynchItemData.Identifier,"The backup does not belong to the root folder");
            foreach(string key in newTreeFlatten.Except(oldTreeFlatten,new SynchItemDataEqualityComparer()).Select(o=>o.Identifier))
                result.Add(key,OperationType.Insert);
            foreach (string key in newTreeFlatten.Join(oldTreeFlatten, n => n.Identifier, o => o.Identifier,
                                (n, o) =>
                                new { Identifier = n.Identifier, Name = n.Name, NewSize = n.Size, OldSize = o.Size }).
                Where(c => c.NewSize != c.OldSize).Select(c => c.Identifier))
                result.Add(key,OperationType.Update);

            foreach (string key in oldTreeFlatten.Except(newTreeFlatten, new SynchItemDataEqualityComparer()).Select(n => n.Identifier))
                result.Add(key,OperationType.Delete);
            return result;
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
