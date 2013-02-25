using System.ServiceModel;
using MySynch.Common;
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
                _serviceHosts.Add(new ServiceHost(changeApplyer));
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
