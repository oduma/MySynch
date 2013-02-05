using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Timers;
using System.Windows;
using MySynch.Common;
using MySynch.Contracts;
using MySynch.Contracts.Messages;
using MySynch.Proxies;

namespace MySynch.Monitor.MVVM.ViewModels
{
    internal class DistributorDetailsViewModel:ViewModelBase
    {
        private System.Windows.Visibility _treeReady;
        public Visibility  TreeReady
        {
            get
            {
                return this._treeReady;
            }

            set
            {
                if (value != _treeReady)
                {
                    _treeReady = value;
                    RaisePropertyChanged("TreeReady");
                }
            }
        }
        private System.Windows.Visibility _treeNotReady;
        public Visibility TreeNotReady
        {
            get
            {
                return this._treeNotReady;
            }

            set
            {
                if (value != _treeNotReady)
                {
                    _treeNotReady = value;
                    RaisePropertyChanged("TreeNotReady");
                }
            }
        }

        public string DistributorName { get; set; }

        private ObservableCollection<PublisherViewModel> _publisherCollection;
        public ObservableCollection<PublisherViewModel> PublisherCollection
        {
            get { return _publisherCollection; }
            set
            {
                if (_publisherCollection != value)
                {
                    _publisherCollection = value;
                    RaisePropertyChanged(() => PublisherCollection);
                }
            }
        }

        private IDistributorMonitorProxy _distributorMonitorProxy;
        private Timer _timer;

        public DistributorDetailsViewModel(IDistributorMonitorProxy distributorMonitorProxy)
        {
            using (LoggingManager.LogMySynchPerformance())
            {
                TreeReady = Visibility.Hidden;
                TreeNotReady = Visibility.Visible;
                _timer= new Timer();
                _timer.Interval = 10000;
                _distributorMonitorProxy = distributorMonitorProxy;
                BackgroundWorker backgroundWorker = new BackgroundWorker();
                backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
                backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
                backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker_ProgressChanged);
                backgroundWorker.RunWorkerAsync();
            }
        }

        void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var abc = e.UserState;
        }

        void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            TreeReady = Visibility.Visible;
            TreeNotReady = Visibility.Hidden;
        }

        void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            GetAllPublishers();
            _timer.Start();
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
        }

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            GetAllPublishers();
        }

        private void GetAllPublishers()
        {
            var distributorInformation = _distributorMonitorProxy.ListAvailableComponentsTree();

            DistributorName = distributorInformation.Name;
            PublisherCollection = new ObservableCollection<PublisherViewModel>();
            foreach (var availablePublisher in distributorInformation.AvailablePublishers)
            {
                AvailableComponent publisher = availablePublisher;
                System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => { PublisherCollection.Add(new PublisherViewModel(publisher)); }));
            }
        }
    }
}
