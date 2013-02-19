using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Timers;
using MySynch.Common;
using MySynch.Contracts.Messages;
using MySynch.Core;
using MySynch.Core.DataTypes;
using MySynch.Core.Interfaces;

namespace MySynch.WindowsService
{
    public partial class MySynchNodeInstance : ServiceBase
    {
        public List<ServiceHost> serviceHosts = new List<ServiceHost>();
        private IDistributor _distributor;
        private string _distributorMapFile;
        private ChangePublisher _changePublisher;
        private string _rootFolder;
        private Timer _timer;
        public MySynchNodeInstance()
        {
            LoggingManager.Debug("Initialize the service");
            _distributor = new Distributor();
            _timer = new Timer();
            _timer.Interval = 60000;
            InitializeComponent();
            ReadTheNodeConfiguration();

            LoggingManager.Debug("Initializion Ok with distribution Map: "+ _distributorMapFile);
        }

        private void ReadTheNodeConfiguration()
        {
            var key = ConfigurationManager.AppSettings.AllKeys.FirstOrDefault(k => k == "DistributorMap");
            if (key == null)
                _distributorMapFile = string.Empty;
            else
                _distributorMapFile = ConfigurationManager.AppSettings[key];
            key = ConfigurationManager.AppSettings.AllKeys.FirstOrDefault(k => k == "LocalPublisherRootFolder");
            if (key == null)
                _rootFolder = string.Empty;
            else
                _rootFolder = ConfigurationManager.AppSettings[key];
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                LoggingManager.Debug("Starting the service...");

                if (serviceHosts != null)
                {
                    serviceHosts.ForEach(CloseServiceHost);
                }

                InitializeDistributor();

                serviceHosts.Add(new ServiceHost(typeof(Subscriber)));
                serviceHosts.Add(new ServiceHost(typeof(RemoteSourceOfData)));

                serviceHosts.ForEach(OpenServiceHost);

                
                InitializeLocalPublisher();
                _timer.Elapsed += timer_Elapsed;
                _timer.Enabled = true;
                _timer.Start();
                //otherwise this is a node that only distributes messages no messages are published from here
                LoggingManager.Debug("Service started.");
                
            }
            catch (Exception ex)
            {
                LoggingManager.LogMySynchSystemError(ex);
                throw;
            }
        }

        private void InitializeLocalPublisher()
        {
            var publisher = _distributor.AvailableChannels.FirstOrDefault(
                c => c.Status == Status.Ok && string.IsNullOrEmpty(c.PublisherInfo.EndpointName)).PublisherInfo.Publisher;

            if (publisher != null && !string.IsNullOrEmpty(_rootFolder))
            {
                _changePublisher = (ChangePublisher)publisher;
                _changePublisher.Initialize(_rootFolder);
                FSWatcher fsWatcher = new FSWatcher(_changePublisher);
                serviceHosts.Add(new ServiceHost(_changePublisher));
            }
        }

        private void InitializeDistributor()
        {
            ComponentResolver componentResolver = new ComponentResolver();
            componentResolver.RegisterAll(new MySynchInstaller());

            _distributor.InitiateDistributionMap(_distributorMapFile, componentResolver);
            serviceHosts.Add(new ServiceHost(_distributor));
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            LoggingManager.Debug("Timer kicked in again.");
            _distributor.DistributeMessages();
        }

        private void OpenServiceHost(ServiceHost serviceHost)
        {
            LoggingManager.Debug("Opened Host: " + serviceHost.BaseAddresses[0].ToString());
            serviceHost.Open();
        }

        private void CloseServiceHost(ServiceHost serviceHost)
        {
            LoggingManager.Debug("Closed Host: " + serviceHost.BaseAddresses[0].ToString());
            serviceHost.Close();
        }

        protected override void OnStop()
        {
            serviceHosts.ForEach(CloseServiceHost);
            LoggingManager.Debug("Service Stopped.");
        }
    }
}
