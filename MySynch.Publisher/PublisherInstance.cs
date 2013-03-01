using System;
using System.ServiceModel;
using MySynch.Common;
using MySynch.Contracts;
using MySynch.Core;
using MySynch.Core.Publisher;

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
                Uri publisherBaseAddress = new Uri(string.Format("http://{0}:8765/publisher/{1}/",
        System.Net.Dns.GetHostName(), Guid.NewGuid().ToString()));
                _serviceHosts.Add(CreateAndConfigureServiceHost<IPublisher>(_changePublisher,publisherBaseAddress));
                Uri dataSourceBaseAddress = new Uri(string.Format("http://{0}:8765/sourceOfData/{1}/",
        System.Net.Dns.GetHostName(), Guid.NewGuid().ToString()));
                _serviceHosts.Add(CreateAndConfigureServiceHost<ISourceOfData,RemoteSourceOfData>(dataSourceBaseAddress));
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
