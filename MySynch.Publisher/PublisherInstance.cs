using System.ServiceModel;
using MySynch.Common;
using MySynch.Core;

namespace MySynch.Publisher
{
    public partial class PublisherInstance : MySynchBaseService
    {
        public PublisherInstance()
        {
            LoggingManager.Debug("Initializing service");
            InitializeComponent();
            ReadTheNodeConfiguration();
            LoggingManager.Debug("Initializion Ok publishing changes from folder: " + _rootFolder);

        }

        protected override void OnStart(string[] args)
        {
            LoggingManager.Debug("Starting service");
            CloseAllServiceHosts();
            InitializeLocalPublisher();
            OpenAllServiceHosts();
            LoggingManager.Debug("Service started.");
        }


        private void InitializeLocalPublisher()
        {
            if (!string.IsNullOrEmpty(_rootFolder))
            {
                var changePublisher = new ChangePublisher();
                changePublisher.Initialize(_rootFolder);
                FSWatcher fsWatcher = new FSWatcher(changePublisher);
                _serviceHosts.Add(new ServiceHost(changePublisher));
                _serviceHosts.Add(new ServiceHost(typeof(RemoteSourceOfData)));
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
