using System.ServiceModel;
using MySynch.Common;
using MySynch.Core;

namespace MySynch.Publisher
{
    public partial class PublisherInstance : MySynchBaseService
    {
        private ChangePublisher _changePublisher;

        public PublisherInstance()
        {
            LoggingManager.Debug("Initializing service");
            _changePublisher = new ChangePublisher();
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
                _changePublisher.Initialize(_rootFolder, new ItemDiscoverer(_rootFolder));
                FSWatcher fsWatcher = new FSWatcher(_changePublisher);
                _serviceHosts.Add(new ServiceHost(_changePublisher));
                _serviceHosts.Add(new ServiceHost(typeof(RemoteSourceOfData)));
            }
        }

        protected override void OnStop()
        {
            LoggingManager.Debug("Stoping service");
            CloseAllServiceHosts();
            _changePublisher.SaveSettingsEndExit();
            LoggingManager.Debug("Service stoped.");
        }
    }
}
