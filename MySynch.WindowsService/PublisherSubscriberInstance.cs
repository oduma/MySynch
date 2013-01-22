using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceProcess;
using MySynch.Core;

namespace MySynch.WindowsService
{
    public partial class PublisherSubscriberInstance : ServiceBase
    {
        public List<ServiceHost> serviceHosts = new List<ServiceHost>();
        public PublisherSubscriberInstance()
        {
            ServiceName = "MySynch.PublisherSubscriberIstance";
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            if (serviceHosts != null)
            {
                serviceHosts.ForEach(CloseServiceHost);
            }

            serviceHosts.Add(new ServiceHost(typeof(ChangePublisher)));
            serviceHosts.Add(new ServiceHost(typeof(ChangeApplyer)));

            serviceHosts.ForEach(OpenServiceHost);
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
