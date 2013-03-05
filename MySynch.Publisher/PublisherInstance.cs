using System;
using MySynch.Common;
using MySynch.Contracts;
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
                _changePublisher.Initialize(_rootFolder, new ItemDiscoverer());
                FSWatcher fsWatcher = new FSWatcher(_changePublisher);
                
                _serviceHosts.Add(CreateAndConfigureServiceHost<IPublisher>(_changePublisher, new Uri(string.Format("http://{0}:{1}/publisher/{2}/",
        System.Net.Dns.GetHostName(), _instancePort, Guid.NewGuid().ToString()))));
                _serviceHosts.Add(CreateAndConfigureServiceHost<ISourceOfData, RemoteSourceOfData>(new Uri(string.Format("http://{0}:{1}/sourceOfData/{2}/",
        System.Net.Dns.GetHostName(), _instancePort, Guid.NewGuid().ToString()))));
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
