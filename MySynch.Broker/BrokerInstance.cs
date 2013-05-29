using System;
using MySynch.Contracts;
using MySynch.Core;
using MySynch.Core.DataTypes;
using Sciendo.Common.IOC;
using Sciendo.Common.Logging;
using Sciendo.Common.WCF;

namespace MySynch.Broker
{
    internal partial class BrokerInstance : WcfHostServiceBase
    {
        private Core.Broker.Broker _broker;
        private StoreType _storeType;


        public BrokerInstance()
        {
            LoggingManager.Debug("Initializing service");
            InitializeComponent();
            _storeType = ConfigurationHelper.ReadBrokerNodeConfiguration();
            LoggingManager.Debug("Initializion will be using StoreName: " + _storeType.StoreName + " of type: " +
                                 _storeType.StoreTypeName);
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
            LoggingManager.Debug("Initializing broker with Store Name:" + _storeType.StoreName + " of type: " +
                                 _storeType.StoreTypeName);
            ComponentResolver componentResolver = new ComponentResolver();
            componentResolver.RegisterAll(new MySynchInstaller());

            try
            {
                _broker = new Core.Broker.Broker(_storeType, componentResolver);

                _serviceHosts.Add(CreateAndConfigureServiceHost<IBroker>(_broker,
                                                                         new Uri(string.Format("http://{0}/{1}/",
                                                                                               System.Net.Dns.
                                                                                                   GetHostName().ToLower(),_storeType.InstanceName))));
                _serviceHosts.Add(CreateAndConfigureServiceHost<IBrokerMonitor>(_broker,new Uri(string.Format("http://{0}/{1}/",
                                                                                               System.Net.Dns.
                                                                                                   GetHostName().ToLower(),_storeType.MonitorInstanceName)),true));
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