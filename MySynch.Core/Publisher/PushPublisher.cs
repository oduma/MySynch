﻿using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.Threading;
using MySynch.Common.Logging;
using MySynch.Common.Serialization;
using MySynch.Contracts;
using MySynch.Contracts.Messages;
using MySynch.Core.DataTypes;
using MySynch.Core.Interfaces;
using MySynch.Proxies.Autogenerated.Interfaces;

namespace MySynch.Core.Publisher
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class PushPublisher:ILocalComponent,IPublisher
    {
        private IBrokerProxy _brokerClient;
        private FSWatcher _fsWatcher;
        internal string HostUrl;
        private object _lock= new object();
        private bool operationInProgress = false;

        #region Initialization Logic

        internal SynchItem CurrentRepository { get; set; }

        public void Initialize(IBrokerProxy brokerClient, LocalComponentConfig localComponentConfiguration, string hostUrl)
        {
            operationInProgress = true;
            if(brokerClient==null)
                throw new ArgumentNullException("brokerClient");
            if (string.IsNullOrEmpty(localComponentConfiguration.RootFolder))
                throw new ArgumentNullException("localComponentConfiguration.RootFolder");
            if(string.IsNullOrEmpty(hostUrl))
                throw new ArgumentNullException("hostUrl");
            LoggingManager.Debug("Initializing publisher with rootfolder: " + localComponentConfiguration.RootFolder);
            HostUrl = hostUrl;
            _brokerClient = brokerClient;
            try
            {
                _fsWatcher = new FSWatcher(localComponentConfiguration.RootFolder, QueueInsert, QueueUpdate, QueueDelete);
                LoggingManager.Debug("Publisher initialized");

            }
            catch (Exception ex)
            {
                LoggingManager.LogMySynchSystemError(ex);
            }
            finally
            {
                operationInProgress = false;
            }
        }
        #endregion

        #region Push Logic
        internal void QueueInsert(string absolutePath)
        {
            LoggingManager.Debug("Will queue an insert for: " + absolutePath);
            ProcessOperation(absolutePath, OperationType.Insert);
        }

        private void ProcessOperation(string absolutePath, OperationType operationType)
        {
            operationInProgress = true;
            try
            {
                if (string.IsNullOrEmpty(absolutePath))
                    return;
                lock (_lock)
                    UpdateCurrentRepository(absolutePath, operationType);
                PublishMessage(absolutePath, operationType);
            }
            catch (Exception ex)
            {
                LoggingManager.LogMySynchSystemError(ex);
                throw;
            }
            finally
            {
                operationInProgress = false;
            }
        }

        private void PublishMessage(string absolutePath, OperationType operationType)
        {
            ReceiveAndDistributeMessageRequest request = new ReceiveAndDistributeMessageRequest
                                                             {
                                                                 PublisherMessage = 
                                                                     new PublisherMessage
                                                                         {
                                                                             AbsolutePath = absolutePath,
                                                                             OperationType = operationType,
                                                                             SourceOfMessageUrl=HostUrl,
                                                                             SourcePathRootName=_fsWatcher.Path 
                                                                         }
                                                             };
            _brokerClient.ReceiveAndDistributeMessage(request);
        }

        internal void QueueUpdate(string absolutePath)
        {
            LoggingManager.Debug("Will queue an update for: " + absolutePath);
            ProcessOperation(absolutePath,OperationType.Update);
        }

        internal void QueueDelete(string absolutePath)
        {
            LoggingManager.Debug("Will queue a delete for: " + absolutePath);
            ProcessOperation(absolutePath,OperationType.Delete);
        }
        #endregion

        public void Close(object backupFileName=null)
        {
            while (operationInProgress)
                Thread.Sleep(500);

            Serializer.SerializeToFile(new List<SynchItem> { CurrentRepository }, (string)backupFileName);
        }

        #region Pull Logic
        public GetDataResponse GetData(GetDataRequest request)
        {
            operationInProgress = true;
            if (request == null || string.IsNullOrEmpty(request.FileName))
                throw new ArgumentNullException("request");
            if (!File.Exists(request.FileName))
                throw new ArgumentException("File does not exist " + request.FileName);
            LoggingManager.Debug("Using remote datasource returning contents of file: " + request.FileName);
            GetDataResponse response = new GetDataResponse();
            FileInfo fInfo = new FileInfo(request.FileName);
            response.Data = new byte[fInfo.Length];
            using (FileStream stream = fInfo.OpenRead())
            {
                stream.Read(response.Data, 0, response.Data.Length);
                stream.Flush();
            }
            operationInProgress = false;
            return response;
        }

        public GetHeartbeatResponse GetHeartbeat()
        {
            return new GetHeartbeatResponse {RootPath = _fsWatcher.Path, Status = true};
        }
        #endregion

        #region Persist traces for offline changes logic

        internal void UpdateCurrentRepository(string absolutePath, OperationType operationType)
        {
            LoggingManager.Debug("Updating CurrentRepository with " + absolutePath);
            if(_fsWatcher==null || string.IsNullOrEmpty(_fsWatcher.Path))
                throw new PublisherSetupException(null,"Publisher not initialized");
            if(CurrentRepository==null)
                CurrentRepository=new SynchItem{SynchItemData = new SynchItemData{Identifier=_fsWatcher.Path,Name=_fsWatcher.Path,Size=0}};
            if (CurrentRepository.Items == null)
                CurrentRepository.Items=new List<SynchItem>();
            switch (operationType)
            {
                case OperationType.Insert:
                    SynchItemManager.AddItem(CurrentRepository, absolutePath, new FileInfo(absolutePath).Length);
                    return;
                case OperationType.Update:
                    SynchItemManager.UpdateExistingItem(CurrentRepository, absolutePath, new FileInfo(absolutePath).Length);
                    return;
                case OperationType.Delete:
                    SynchItemManager.DeleteItem(CurrentRepository, absolutePath);
                    return;

            }
        }



        #endregion
    }
}
