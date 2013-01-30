using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using MySynch.Core;
using MySynch.Core.Interfaces;

namespace MySynch.WindowsService
{
    public partial class PublisherSubscriberInstance : ServiceBase
    {
        public List<ServiceHost> serviceHosts = new List<ServiceHost>();
        private IDistributor _distributor;
        private string _distributorMapFile;

        public PublisherSubscriberInstance()
        {
            ServiceName = "MySynch.PublisherSubscriberIstance";
            _distributor = new Distributor();
            InitializeComponent();
            var key = ConfigurationManager.AppSettings.AllKeys.FirstOrDefault(k => k == "DistributorMap");
            if (key == null)
                _distributorMapFile = string.Empty;
            else
                _distributorMapFile = ConfigurationManager.AppSettings[key];
        }

        protected override void OnStart(string[] args)
        {
            if (serviceHosts != null)
            {
                serviceHosts.ForEach(CloseServiceHost);
            }

            serviceHosts.Add(new ServiceHost(typeof(ChangePublisher)));
            serviceHosts.Add(new ServiceHost(typeof(ChangeApplyer)));
            serviceHosts.Add(new ServiceHost(_distributor));

            serviceHosts.ForEach(OpenServiceHost);
            
            ComponentResolver componentResolver=new ComponentResolver();
            componentResolver.RegisterAll(new MySynchInstaller());
            _distributor.InitiateDistributionMap(_distributorMapFile, componentResolver);
        }

        private void OpenServiceHost(ServiceHost serviceHost)
        {
            serviceHost.Open();
        }

        private void CloseServiceHost(ServiceHost serviceHost)
        {
            serviceHost.Close();
        }

        protected override void OnStop()
        {
            serviceHosts.ForEach(CloseServiceHost);
        }
    }
}
