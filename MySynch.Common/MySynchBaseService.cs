using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;

namespace MySynch.Common
{
    public abstract class MySynchBaseService: ServiceBase
    {
        protected string _rootFolder;
        protected string _distributorMapFile;

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
                _distributorMapFile = string.Empty;
            else
                _distributorMapFile = ConfigurationManager.AppSettings[key];

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


    }
}
