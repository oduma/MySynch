using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Timers;
using System.Windows.Input;
using MySynch.Contracts.Messages;
using MySynch.Monitor.Utils;
using MySynch.Proxies.Interfaces;

namespace MySynch.Monitor.MVVM.ViewModels
{
    internal class MonitorViewModel:AsynchViewModelBase
    {
        private IDistributorMonitorProxy _distributorMonitorProxy;

        private string _distributorName;
        public string DistributorName
        {
            get
            {
                return this._distributorName;
            }

            set
            {
                if (value != _distributorName)
                {
                    _distributorName = value;
                    RaisePropertyChanged("DistributorName");
                }
            }
        }

        private Timer _timer;
        private int _localDistributorPort;

        public ICommand Reevaluate { get; private set; }

        private ObservableCollection<AvailableChannelViewModel> _availableChannels;

        public ObservableCollection<AvailableChannelViewModel> AvailableChannels
        {
            get { return _availableChannels; }
            set
            {
                if (_availableChannels != value)
                {
                    _availableChannels = value;
                    RaisePropertyChanged(() => AvailableChannels);
                }
            }
        }


        public MonitorViewModel(IDistributorMonitorProxy distributorMonitorProxy,ListAvailableChannelsResponse listAvailableComponentsTreeResponse, int localDistributorPort)
        {
            _localDistributorPort = localDistributorPort;
            _distributorMonitorProxy = distributorMonitorProxy;
            DistributorName = listAvailableComponentsTreeResponse.Name;
            var availableChannels = listAvailableComponentsTreeResponse.Channels;
            AvailableChannels = new ObservableCollection<AvailableChannelViewModel>();
            AvailableChannels = availableChannels.AddToChannels(AvailableChannels);
            
            _timer = new Timer();
            _timer.Interval = 10000;

        }

        public void InitiateView()
        {
            _timer.Start();
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
            Reevaluate = new RelayCommand(PerformReevaluate);
        }

        private void PerformReevaluate()
        {
            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += DoWork;
            backgroundWorker.RunWorkerCompleted += DoWorkCompleted;
            backgroundWorker.ProgressChanged += DoWorkProgressChanged;
            backgroundWorker.RunWorkerAsync();
            _timer.Enabled = false;
            BlockTheUI(DoWorkProgressChanged);
        }

        private void DoWorkProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            WorkingMessage = e.UserState.ToString();

        }

        private void DoWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            UnblockTheUI();
            _timer.Enabled = true;
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            DoWorkProgressChanged(this,new ProgressChangedEventArgs(0,"Reevaluating all channels in the distributor."));
            DoWorkProgressChanged(this, new ProgressChangedEventArgs(0,ServiceHelper.StopDistributor()));
            DoWorkProgressChanged(this, new ProgressChangedEventArgs(0,"Starting distributor..."));
            DoWorkProgressChanged(this, new ProgressChangedEventArgs(0, ServiceHelper.StartDistributor()));
            var clienHelper = new ClientHelper();
            clienHelper.DisconnectFromADistributor(DoWorkProgressChanged,_localDistributorPort,_distributorMonitorProxy);
            ListAvailableChannelsResponse availableChannels= new ListAvailableChannelsResponse();
            clienHelper.ConnectToADistributor(DoWorkProgressChanged, _localDistributorPort, out _distributorMonitorProxy,
                                              out availableChannels);
            DoWorkProgressChanged(this, new ProgressChangedEventArgs(0, "Listing all channels in the distributor."));
            DistributorName = availableChannels.Name;
            AvailableChannels = new ObservableCollection<AvailableChannelViewModel>();
            AvailableChannels = availableChannels.Channels.AddToChannels(AvailableChannels);

        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Enabled = false;
            var availableChannels = _distributorMonitorProxy.ListAvailableChannels();
            AvailableChannels = availableChannels.Channels.AddToChannels(AvailableChannels);
            _timer.Enabled = true;
        }
    }
}
