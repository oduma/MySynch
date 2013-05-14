using System;
using System.Collections.Generic;
using System.IO;
using MySynch.Common.Logging;
using MySynch.Contracts;
using MySynch.Contracts.Messages;
using MySynch.Core;
using MySynch.Core.Publisher;

namespace MySynch.Publisher
{
    public partial class PublisherInstance : MySynchServiceBase
    {

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
            CloseAllServiceHosts();
            if(InitializeLocalComponent())
                StartTimer(60000,TimerElapseMethod);
            OpenAllServiceHosts();
            LoggingManager.Debug("Service started.");
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
            LocalComponent.Close();
            DetachFromBroker();
            LoggingManager.Debug("Service stoped.");
        }

    }
}
