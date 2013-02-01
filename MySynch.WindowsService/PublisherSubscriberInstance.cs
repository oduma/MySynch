using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using MySynch.Contracts.Messages;
using MySynch.Core;
using MySynch.Core.Interfaces;
using MySynch.Core.Utilities;

namespace MySynch.WindowsService
{
    public partial class PublisherSubscriberInstance : ServiceBase
    {
        public List<ServiceHost> serviceHosts = new List<ServiceHost>();
        private IDistributor _distributor;
        private string _distributorMapFile;
        private ChangePublisher _changePublisher;
        private string _rootFolder;

        public PublisherSubscriberInstance()
        {
            LoggingManager.Debug("Initialize the service");
            ServiceName = "MySynch.PublisherSubscriberIstance";
            _distributor = new Distributor();
            InitializeComponent();
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

            LoggingManager.Debug("Initializion Ok with distribution Map: "+ _distributorMapFile);
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

                serviceHosts.Add(new ServiceHost(typeof(ChangePublisher)));
                serviceHosts.Add(new ServiceHost(typeof(ChangeApplyer)));
                serviceHosts.Add(new ServiceHost(_distributor));

                serviceHosts.ForEach(OpenServiceHost);

                ComponentResolver componentResolver = new ComponentResolver();
                componentResolver.RegisterAll(new MySynchInstaller());

                _distributor.InitiateDistributionMap(_distributorMapFile, componentResolver);
                
                var publishers = _distributor.AvailableChannels.Where(
                    c => c.Status == Status.Ok && string.IsNullOrEmpty(c.PublisherInfo.EndpointName)).Select(
                        c => c.PublisherInfo.Publisher);
                if (publishers.Count() != 0 && !string.IsNullOrEmpty(_rootFolder))
                {
                    _changePublisher = (ChangePublisher)publishers.First();
                    _changePublisher.Initialize(_rootFolder);
                    FSWatcher fsWatcher = new FSWatcher(_changePublisher);
                    
                }
                //otherwise this is a node that only distributes messages no messages are published from here
                LoggingManager.Debug("Service started.");
                
            }
            catch (Exception ex)
            {
                LoggingManager.LogMySynchSystemError(ex);
                throw;
            }
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
