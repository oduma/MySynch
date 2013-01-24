using System;
using System.Collections.Generic;
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
            _temporaryStore=new SortedList<string, OperationType>();
        }

        public event EventHandler<ItemsQueuedEventArgs> ItemsQueued;

        public void QueueInsert(string absolutePath)
        {
            if (string.IsNullOrEmpty(absolutePath))
                return;
            lock (_lock)
            {
                WriteIfNew(absolutePath, OperationType.Insert);
            }
        }

        private void WriteIfNew(string absolutePath, OperationType operationType)
        {
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
                WriteIfNew(absolutePath, OperationType.Update);
            }
        }

        public void QueueDelete(string absolutePath)
        {
            if (string.IsNullOrEmpty(absolutePath))
                return;
            lock (_lock)
            {
                WriteIfNew(absolutePath, OperationType.Delete);
            }
        }

        public void Initialize(string rootFolder)
        {
            if(string.IsNullOrEmpty(rootFolder))
                throw new ArgumentNullException("rootFolder");
            _sourceRootName = rootFolder;
        }

        public ChangePushPackage PublishPackage()
        {
            if(string.IsNullOrEmpty(_sourceRootName) || _temporaryStore==null)
                throw new PublisherSetupException(_sourceRootName,"Publisher Information incomplete");
            lock (_lock)
            {
                ChangePushPackage changePushPackage = new ChangePushPackage
                                                          {Source = Environment.MachineName, SourceRootName = _sourceRootName};
                List<ChangePushItem> _pushItems= new List<ChangePushItem>();
                foreach(string key in _temporaryStore.Keys)
                    _pushItems.Add(new ChangePushItem{AbsolutePath = key,OperationType = _temporaryStore[key]});
                _temporaryStore.Clear();
                changePushPackage.ChangePushItems = _pushItems;
                return changePushPackage;
            }
        }

        public void RemovePackage(ChangePushPackage packagePublished)
        {
            throw new NotImplementedException();
        }

        public string MachineName
        {
            get { return Environment.MachineName; }
        }

        public HeartbeatResponse GetHeartbeat()
        {
            return new HeartbeatResponse {Status = true};
        }
    }
}
