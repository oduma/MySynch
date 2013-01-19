using System;
using System.Collections.Generic;
using MySynch.Core.DataTypes;
using MySynch.Core.Interfaces;

namespace MySynch.Core
{
    public class ChangePublisher: IChangePublisher
    {
        private SortedList<string,OperationType> _temporaryStore;
        private object _lock= new object();
        private string _source;
        private string _sourceRootName;

        public ChangePublisher(string source,string sourceRootName)
        {
            _temporaryStore=new SortedList<string, OperationType>();
            _source = source;
            _sourceRootName = sourceRootName;
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

        public ChangePushPackage PublishPackage()
        {
            if(string.IsNullOrEmpty(_source) || string.IsNullOrEmpty(_sourceRootName) || _temporaryStore==null)
                throw new PublisherSetupException(_source,_sourceRootName,"Publisher Information incomplete");
            lock (_lock)
            {
                ChangePushPackage changePushPackage = new ChangePushPackage
                                                          {Source = _source, SourceRootName = _sourceRootName};
                List<ChangePushItem> _pushItems= new List<ChangePushItem>();
                foreach(string key in _temporaryStore.Keys)
                    _pushItems.Add(new ChangePushItem{AbsolutePath = key,OperationType = _temporaryStore[key]});
                _temporaryStore.Clear();
                changePushPackage.ChangePushItems = _pushItems;
                return changePushPackage;
            }
        }
    }
}
