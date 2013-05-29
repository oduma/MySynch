using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using MySynch.Contracts;
using MySynch.Contracts.Messages;
using MySynch.Core;
using MySynch.Core.Configuration;
using Sciendo.Common.IOC;
using Sciendo.Common.Logging;

namespace MySynch.Subscriber
{
    public partial class SubscriberInstance : MySynchServiceBase
    {
        public SubscriberInstance()
        {
            LoggingManager.Debug("Initializing service");
            LocalComponentConfig = ConfigurationManager.GetSection("mySynchSubscriberConfiguration") as MySynchLocalComponentConfigurationSection;
            HostUrl = string.Format("http://{0}/{1}/",
            System.Net.Dns.
                GetHostName().ToLower(),LocalComponentConfig.InstanceName);
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
                        ServiceRole = ServiceRole.Subscriber,
                        ServiceUrl = HostUrl
                    }
            };

            InitializeComponent();
            LoggingManager.Debug("Will Initialize subscribing changes to folder: " + LocalComponentConfig.LocalRootFolder);
        }

        protected override void OnStart(string[] args)
        {
            LoggingManager.Debug("Starting service");
            CloseAllServiceHosts();
            if (InitializeLocalComponent())
                StartTimer(500, TimerElapseMethod);
            OpenAllServiceHosts();
            LoggingManager.Debug("Service started.");

        }

        public override bool InitializeLocalComponent()
        {
            if (string.IsNullOrEmpty(LocalComponentConfig.LocalRootFolder) || !Directory.Exists(LocalComponentConfig.LocalRootFolder))
                return false;
            ComponentResolver componentResolver=new ComponentResolver();
            componentResolver.RegisterAll(new MySynchInstaller());
            LocalComponent = new Core.Subscriber.Subscriber(componentResolver);

            _serviceHosts.Add(CreateAndConfigureServiceHost<ISubscriber>((ISubscriber)LocalComponent,
                                                                     new Uri(HostUrl)));
            LoggingManager.Debug("Subscriber Initialized.");
            
            return true;

        }

        protected override void OnStop()
        {
            LoggingManager.Debug("Stoping service");
            CloseAllServiceHosts();
            DetachFromBroker();
            LocalComponent.Close();
            LoggingManager.Debug("Service stoped.");

        }
    }
}
