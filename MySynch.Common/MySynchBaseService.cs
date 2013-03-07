using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.ServiceProcess;

namespace MySynch.Common
{
    public abstract class MySynchBaseService: ServiceBase
    {
        protected string _rootFolder;
        protected string _distributorMapFile;
        protected int _instancePort;

        protected List<ServiceHost> _serviceHosts = new List<ServiceHost>();

        protected void ReadTheNodeConfiguration()
        {
            var key = ConfigurationManager.AppSettings.AllKeys.FirstOrDefault(k => k == "LocalRootFolder");
            if (key == null)
                _rootFolder = string.Empty;
            else
                _rootFolder = ConfigurationManager.AppSettings[key];
            key = ConfigurationManager.AppSettings.AllKeys.FirstOrDefault(k => k == "DistributorMap");
            if (key == null)
                _distributorMapFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,@"map\distributormap.xml");
            else
                _distributorMapFile = ConfigurationManager.AppSettings[key];

            key = ConfigurationManager.AppSettings.AllKeys.FirstOrDefault(k => k == "InstancePort");
            if (key == null)
                _instancePort = 0;
            else
                _instancePort = Convert.ToInt32(ConfigurationManager.AppSettings[key]);

        }

        protected void CloseAllServiceHosts()
        {
            if (_serviceHosts != null)
            {
                _serviceHosts.ForEach(CloseServiceHost);
            }

        }

        protected void OpenAllServiceHosts()
        {
            _serviceHosts.ForEach(OpenServiceHost);
        }

        private void OpenServiceHost(ServiceHost serviceHost)
        {
            serviceHost.Open();
            LoggingManager.Debug("Opened Host: " + serviceHost.BaseAddresses[0].ToString());
        }

        private void CloseServiceHost(ServiceHost serviceHost)
        {
            LoggingManager.Debug("Closed Host: " + serviceHost.BaseAddresses[0].ToString());
            serviceHost.Close();
        }


        protected ServiceHost CreateAndConfigureServiceHost<TContract,TInstance>(Uri baseAddress)
        {
            var serviceHost = new ServiceHost(typeof (TInstance),baseAddress);
            
            var serviceEndPoint = serviceHost.AddServiceEndpoint(typeof(TContract), ClientServerBindingHelper.GetBinding(),string.Empty);
            serviceEndPoint.Behaviors.Add(new MySynchAuditBehavior());

            // ** DISCOVERY ** //
            // make the service discoverable by adding the discovery behavior
            ServiceDiscoveryBehavior serviceDiscoveryBehavior=new ServiceDiscoveryBehavior();
            serviceHost.Description.Behaviors.Add(serviceDiscoveryBehavior);

            // send announcements on UDP multicast transport
            serviceDiscoveryBehavior.AnnouncementEndpoints.Add(
              new UdpAnnouncementEndpoint());
    
            // ** DISCOVERY ** //
            // add the discovery endpoint that specifies where to publish the services
            serviceHost.AddServiceEndpoint(new UdpDiscoveryEndpoint());


            return serviceHost;
        }

        protected ServiceHost CreateAndConfigureServiceHost<T>(T serviceInstance, Uri baseAddress)
        {
            var serviceHost = new ServiceHost(serviceInstance, baseAddress);
            var serviceEndPoint = serviceHost.AddServiceEndpoint(typeof(T), ClientServerBindingHelper.GetBinding(), string.Empty);

            serviceEndPoint.Behaviors.Add(new MySynchAuditBehavior());

            // ** DISCOVERY ** //
            // make the service discoverable by adding the discovery behavior
            ServiceDiscoveryBehavior serviceDiscoveryBehavior = new ServiceDiscoveryBehavior();
            serviceHost.Description.Behaviors.Add(serviceDiscoveryBehavior);

            // send announcements on UDP multicast transport
            serviceDiscoveryBehavior.AnnouncementEndpoints.Add(
              new UdpAnnouncementEndpoint());

            // ** DISCOVERY ** //
            // add the discovery endpoint that specifies where to publish the services
            serviceHost.AddServiceEndpoint(new UdpDiscoveryEndpoint());

            return serviceHost;
        }


    }
}
