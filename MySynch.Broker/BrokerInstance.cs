using System;
using System.Configuration;
using System.Linq;
using MySynch.Common;
using MySynch.Common.Logging;
using MySynch.Common.Serialization;
using MySynch.Contracts;
using MySynch.Core;

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
            _storeType = Helper.ReadTheNodeConfiguration();
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
            MySynchComponentResolver componentResolver = new MySynchComponentResolver();
            componentResolver.RegisterAll(new MySynchInstaller());

            try
            {
                _broker = new Core.Broker.Broker(_storeType, componentResolver);

                _serviceHosts.Add(CreateAndConfigureServiceHost<IBroker>(_broker,
                                                                         new Uri(string.Format("http://{0}/broker/",
                                                                                               System.Net.Dns.
                                                                                                   GetHostName().ToLower()))));
                LoggingManager.Debug("Broker initialized.");
            }
            catch (Exception ex)
            {
                LoggingManager.LogMySynchSystemError(ex);
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