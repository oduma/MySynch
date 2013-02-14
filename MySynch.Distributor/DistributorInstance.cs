using System.ServiceModel;
using System.Timers;
using MySynch.Common;
using MySynch.Core;

namespace MySynch.Distributor
{
    public partial class DistributorInstance : MySynchBaseService
    {
        private Core.Distributor _distributor;
        private Timer _timer;

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
            _distributor = new Core.Distributor();
            _timer = new Timer();
            _timer.Interval = 60000;
            ComponentResolver componentResolver = new ComponentResolver();
            componentResolver.RegisterAll(new MySynchInstaller());

            _distributor.InitiateDistributionMap(_distributorMapFile, componentResolver);
            _timer.Elapsed += timer_Elapsed;
            _timer.Enabled = true;
            _timer.Start();

            _serviceHosts.Add(new ServiceHost(_distributor));

        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            LoggingManager.Debug("Timer kicked in again.");
            _distributor.DistributeMessages();
        }

        protected override void OnStop()
        {
            LoggingManager.Debug("Stoping service");
            CloseAllServiceHosts();
            LoggingManager.Debug("Service stoped.");
        }
    }
}
