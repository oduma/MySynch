using System;
using System.ServiceModel;
using MySynch.Common;
using MySynch.Common.Logging;
using MySynch.Contracts;
using MySynch.Core;

namespace MySynch.Subscriber
{
    public partial class SubscriberInstance : MySynchBaseService
    {
        public SubscriberInstance()
        {
            LoggingManager.Debug("Initializing service");
            InitializeComponent();
            ReadTheNodeConfiguration();
            LoggingManager.Debug("Initializion Ok subscriber.");
        }

        protected override void OnStart(string[] args)
        {
            LoggingManager.Debug("Starting service");
            CloseAllServiceHosts();
            InitializeLocalSubscriber();
            OpenAllServiceHosts();
            LoggingManager.Debug("Service started.");

        }

        private void InitializeLocalSubscriber()
        {
            if (!string.IsNullOrEmpty(_rootFolder))
            {
                LoggingManager.Debug("Initializing Subscriber with "+ _rootFolder);
                var changeApplyer = new Core.Subscriber.Subscriber();
                changeApplyer.Initialize(_rootFolder);
                _serviceHosts.Add(CreateAndConfigureServiceHost<ISubscriber>(changeApplyer, new Uri(string.Format("http://{0}:{1}/subscriber/{2}/",
        System.Net.Dns.GetHostName(), _instancePort, Guid.NewGuid().ToString())),true));
                LoggingManager.Debug("Subscriber Initialized.");
            }
        }

        protected override void OnStop()
        {
            LoggingManager.Debug("Stoping service");
            CloseAllServiceHosts();
            LoggingManager.Debug("Service stoped.");

        }
    }
}
