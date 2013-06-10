using System;
using System.Configuration;
using MySynch.Contracts;
using MySynch.Core;
using MySynch.Core.Configuration;
using Sciendo.Common.IOC;
using Sciendo.Common.Logging;
using Sciendo.Common.WCF;

namespace MySynch.Broker
{
    internal partial class BrokerInstance : WcfHostServiceBase
    {
        private Core.Broker.Broker _broker;
        private MySynchBrokerConfigurationSection _brokerConfiguration;


        public BrokerInstance()
        {
            LoggingManager.Debug("Initializing service");
            InitializeComponent();
            _brokerConfiguration = ConfigurationManager.GetSection("mySynchBrokerConfiguration") as MySynchBrokerConfigurationSection;
            LoggingManager.Debug("Initializion will be using StoreName: " + _brokerConfiguration.StoreName + " of type: " +
                                 _brokerConfiguration.StoreType);
        }

        protected override void OnStart(string[] args)
        {
            LoggingManager.Debug("Starting service");
            CloseAllServiceHosts();
            InitializeBroker();
            OpenAllServiceHosts();
            LoggingManager.Debug("Service started.");
        }

        private void InitializeBroker()
        {
            LoggingManager.Debug("Initializing broker with Store Name:" + _brokerConfiguration.StoreName + " of type: " +
                                 _brokerConfiguration.StoreType);
            ComponentResolver componentResolver = new ComponentResolver();
            componentResolver.RegisterAll(new MySynchInstaller());

            try
            {
                _broker = new Core.Broker.Broker(_brokerConfiguration, componentResolver);

                _serviceHosts.Add(CreateAndConfigureServiceHost<IBroker>(_broker,
                                                                         new Uri(string.Format("http://{0}/{1}/",
                                                                                               System.Net.Dns.
                                                                                                   GetHostName().ToLower(),_brokerConfiguration.InstanceName))));
                _serviceHosts.Add(CreateAndConfigureServiceHost<IBrokerMonitor>(_broker,new Uri(string.Format("http://{0}/{1}/",
                                                                                               System.Net.Dns.
                                                                                                   GetHostName().ToLower(),_brokerConfiguration.MonitorInstanceName)),true));
                LoggingManager.Debug("Broker initialized.");
            }
            catch (Exception ex)
            {
                LoggingManager.LogSciendoSystemError(ex);
                LoggingManager.Debug("Broker not initialized.");
            }
        }


        protected override void OnStop()
        {
            LoggingManager.Debug("Stoping service");
            CloseAllServiceHosts();
            LoggingManager.Debug("Service stoped.");
        }
    }
}