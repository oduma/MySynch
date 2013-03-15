using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using MySynch.Proxies;
using MySynch.Proxies.Interfaces;

namespace MySynch.Monitor.MVVM.ViewModels
{
    internal class MonitorViewModel:ViewModelBase
    {
        private IDistributorMonitorProxy _distributorMonitorProxy;
        private int _localDsitributorPort;

        public MonitorViewModel()
        {
            _distributorMonitorProxy = new DistributorMonitorClient();

            var key = ConfigurationManager.AppSettings.AllKeys.FirstOrDefault(k => k == "LocalDistributorPort");
            if (key != null)
                _localDsitributorPort= Convert.ToInt32(ConfigurationManager.AppSettings[key]);
            else
                _localDsitributorPort = 0;
        }

        public void InitiateView()
        {
            _distributorMonitorProxy.InitiateUsingPort(_localDsitributorPort);
            //_distributorMonitorProxy.
        }
    }
}
