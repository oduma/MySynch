using System;
using System.ServiceModel;
using System.Timers;
using MySynch.Common;
using MySynch.Common.Logging;
using MySynch.Contracts;
using MySynch.Core;
using MySynch.Core.DataTypes;

namespace MySynch.Distributor
{
    public partial class DistributorInstance : MySynchBaseService
    {
        private Core.Distributor.Distributor _distributor;
        private Timer _timer;
        private int _noOfSpins = 0;

        public DistributorInstance()
        {
            LoggingManager.Debug("Initializing service");
            InitializeComponent();
            ReadTheNodeConfiguration();
            LoggingManager.Debug("Initializion Ok distributor using map: " + _distributorMapFile);

        }

        protected override void OnStart(string[] args)
        {
            LoggingManager.Debug("Starting service");
            CloseAllServiceHosts();
            InitializeDistributor();
            OpenAllServiceHosts();
            LoggingManager.Debug("Service started.");

        }

        private void InitializeDistributor()
        {
            LoggingManager.Debug("Initializing distributor with map:" + _distributorMapFile);
            _distributor = new Core.Distributor.Distributor();
            _timer = new Timer();
            _timer.Interval = 60000;
            MySynchComponentResolver componentResolver = new MySynchComponentResolver();
            componentResolver.RegisterAll(new MySynchInstaller());

            _distributor.InitiateDistributionMap(_distributorMapFile, componentResolver);
            _timer.Elapsed += timer_Elapsed;
            _timer.Enabled = true;
            _timer.Start();

            _serviceHosts.Add(CreateAndConfigureServiceHost<IDistributorMonitor>(_distributor, new Uri(string.Format("http://{0}:{1}/distributor/{2}/",
    System.Net.Dns.GetHostName(), _instancePort, Guid.NewGuid().ToString()))));
            LoggingManager.Debug("Distributor initialized.");

        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            LoggingManager.Debug("Timer kicked in again.");
            _timer.Enabled = false;
            if (_noOfSpins == 10)
            {
                _noOfSpins = 0;
                //[TODO:] will have to have it working at one moment
                //_distributor.ReEvaluateAllChannels();
            }
            _distributor.DistributeMessages();
            _noOfSpins++;
            LoggingManager.Debug("Finished distribution round. NoOfSpins: " +_noOfSpins);
            _timer.Enabled = true;
            LoggingManager.Debug("Starting timer again.");
        }

        protected override void OnStop()
        {
            LoggingManager.Debug("Stoping service");
            CloseAllServiceHosts();
            LoggingManager.Debug("Service stoped.");
        }
    }
}
