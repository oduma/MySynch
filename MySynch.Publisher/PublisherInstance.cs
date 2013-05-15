using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using MySynch.Common.Logging;
using MySynch.Contracts;
using MySynch.Contracts.Messages;
using MySynch.Core;
using MySynch.Core.Publisher;

namespace MySynch.Publisher
{
    public partial class PublisherInstance : MySynchServiceBase
    {
        private bool _firstTimeRunningAfterRestart;
        private readonly string _backupFileName = AppDomain.CurrentDomain.BaseDirectory + "backup.xml";


        public PublisherInstance()
        {
            LoggingManager.Debug("Initializing service");
            CurrentAttachRequest = new AttachRequest
                                         {
                                             RegistrationRequest =
                                                 new Registration
                                                     {
                                                         MessageMethod = "temp placeholder",
                                                         OperationTypes =
                                                             new List<OperationType>
                                                                 {
                                                                     OperationType.Delete,
                                                                     OperationType.Insert,
                                                                     OperationType.Update
                                                                 },
                                                         ServiceRole = ServiceRole.Publisher,
                                                         ServiceUrl = HostUrl
                                                     }
                                         };
            LocalComponent = new PushPublisher();
            InitializeComponent();
            HostUrl = string.Format("http://{0}/publisher/",
                                    System.Net.Dns.
                                        GetHostName().ToLower());
            LoggingManager.Debug("Will Initialize publishing changes from folder: " + LocalComponentConfig.RootFolder);
        }


        protected override void OnStart(string[] args)
        {
            LoggingManager.Debug("Starting service");
            _firstTimeRunningAfterRestart = true;
            CloseAllServiceHosts();
            if(InitializeLocalComponent())
                StartTimer(60000,TimerElapseMethod);
            OpenAllServiceHosts();
            LoggingManager.Debug("Service started.");
        }

        protected override void TimerElapseMethod(object sender, ElapsedEventArgs e)
        {
            LoggingManager.Debug("Timer kicked in again.");
            Timer.Enabled = false;
            if (TryMakeComponentKnown(LocalComponentConfig.BrokerName))
            {
                if (_firstTimeRunningAfterRestart)
                {
                    OfflineChangesDetector.ForcePublishAllOfflineChanges((PushPublisher) LocalComponent,_backupFileName,
                                                                         LocalComponentConfig.RootFolder);
                    _firstTimeRunningAfterRestart = false;
                }
                Timer.Interval = 120000;
                Timer.Enabled = true;
                return;
            }
            Timer.Interval = 60000;
            Timer.Enabled = true;
            LoggingManager.Debug("Starting timer again.");

        }

        public override bool InitializeLocalComponent()
        {
            if (string.IsNullOrEmpty(LocalComponentConfig.RootFolder) || !Directory.Exists(LocalComponentConfig.RootFolder))
                return false;
            _serviceHosts.Add(CreateAndConfigureServiceHost<IPublisher>((IPublisher)LocalComponent,
                                                                     new Uri(HostUrl)));
            return true;
        }

        protected override void OnStop()
        {
            LoggingManager.Debug("Stoping service");
            CloseAllServiceHosts();
            LocalComponent.Close(_backupFileName);
            DetachFromBroker();
            LoggingManager.Debug("Service stoped.");
        }

    }
}
