using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.ServiceProcess;
using System.Timers;
using MySynch.Contracts;
using MySynch.Contracts.Messages;
using MySynch.Core;
using MySynch.Core.Configuration;
using MySynch.Core.Publisher;
using Sciendo.Common.Logging;

namespace MySynch.Publisher
{
    public partial class PublisherInstance : MySynchServiceBase
    {
        private bool _firstTimeRunningAfterRestart;
        private readonly string _backupFileName = AppDomain.CurrentDomain.BaseDirectory + "backup.xml";


        public PublisherInstance()
        {
            LocalComponentConfig = ConfigurationManager.GetSection("mySynchPublisherConfiguration") as MySynchPublisherConfigurationSection;

            LoggingManager.Debug("Initializing service");
            HostUrl = string.Format("http://{0}/{1}/",
                        System.Net.Dns.
                            GetHostName().ToLower(),LocalComponentConfig.InstanceName);
            MonitorHostUrl = string.Format("http://{0}/{1}/",
                System.Net.Dns.GetHostName().ToLower(), LocalComponentConfig.MonitorInstanceName);

            CurrentAttachRequest = new AttachRequest
                                         {
                                             RegistrationRequest =
                                                 new Registration
                                                     {
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
            LoggingManager.Debug("Will Initialize publishing changes from folder: " + LocalComponentConfig.LocalRootFolder);
        }


        protected override void OnStart(string[] args)
        {
            LoggingManager.Debug("Starting service");
            _firstTimeRunningAfterRestart = true;
            CloseAllServiceHosts();
            if(InitializeLocalComponent())
                StartTimer(500,TimerElapseMethod);
            OpenAllServiceHosts();
            LoggingManager.Debug("Service started.");
        }

        protected override void TimerElapseMethod(object sender, ElapsedEventArgs e)
        {
            LoggingManager.Debug("Timer kicked in again.");
            Timer.Enabled = false;
            if (TryMakeComponentKnown(LocalComponentConfig.BrokerUrl))
            {
                if (_firstTimeRunningAfterRestart && ((MySynchPublisherConfigurationSection)LocalComponentConfig).ConsiderOfflineChanges.ToLower()=="yes")
                {
                    OfflineChangesDetector.ForcePublishAllOfflineChanges((PushPublisher) LocalComponent,_backupFileName,
                                                                         LocalComponentConfig.LocalRootFolder);
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
            if (string.IsNullOrEmpty(LocalComponentConfig.LocalRootFolder) || !Directory.Exists(LocalComponentConfig.LocalRootFolder))
                return false;
            _serviceHosts.Add(CreateAndConfigureServiceHost<IPublisher>((IPublisher)LocalComponent,
                                                                     new Uri(HostUrl)));
            _serviceHosts.Add(CreateAndConfigureServiceHost<IComponentMonitor>(LocalComponent, new Uri(MonitorHostUrl), true));

            return true;
        }

        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            if (powerStatus == PowerBroadcastStatus.Suspend)
            {
                LoggingManager.Debug("Suspending service");
                CloseAllServiceHosts();
                LocalComponent.Close(_backupFileName);
                DetachFromBroker();
                LoggingManager.Debug("Service suspended.");
                return base.OnPowerEvent(powerStatus);
            }
            if (powerStatus == PowerBroadcastStatus.ResumeSuspend)
            {
                LoggingManager.Debug("Restarting service");
                _firstTimeRunningAfterRestart = true;
                CloseAllServiceHosts();
                if (InitializeLocalComponent())
                    StartTimer(500, TimerElapseMethod);
                OpenAllServiceHosts();
                LoggingManager.Debug("Service restarted.");
                return base.OnPowerEvent(powerStatus);

           }
            return base.OnPowerEvent(powerStatus);
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
