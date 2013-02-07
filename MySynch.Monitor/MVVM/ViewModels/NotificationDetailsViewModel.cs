﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Timers;
using System.Windows;
using MySynch.Common;
using MySynch.Contracts.Messages;
using MySynch.Proxies;

namespace MySynch.Monitor.MVVM.ViewModels
{
    internal class NotificationDetailsViewModel:ViewModelBase
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

        public string Name { get; set; }

        public string Type { get; set; }

        public string State { get; set; }

        private ObservableCollection<NotificationDetailsViewModel> _notificationDetailsCollection;
        public ObservableCollection<NotificationDetailsViewModel> NotificationDetailsCollection
        {
            get { return _notificationDetailsCollection; }
            set
            {
                if (_notificationDetailsCollection != value)
                {
                    _notificationDetailsCollection = value;
                    RaisePropertyChanged(() => NotificationDetailsCollection);
                }
            }
        }

        private IDistributorMonitorProxy _distributorMonitorProxy;
        private Timer _timer;

        public NotificationDetailsViewModel()
        {
            
        }
        public NotificationDetailsViewModel(IDistributorMonitorProxy distributorMonitorProxy)
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

            //a bit of test for the packages
            distributorInformation.AvailablePublishers[0].Packages= new List<Package>();
            distributorInformation.AvailablePublishers[0].Packages.Add(new Package{Id=Guid.NewGuid(),State=Contracts.Messages.State.Published});
            distributorInformation.AvailablePublishers[0].Packages.Add(new Package { Id = Guid.NewGuid(), State = Contracts.Messages.State.Published });
            distributorInformation.AvailablePublishers[0].Packages.Add(new Package { Id = Guid.NewGuid(), State = Contracts.Messages.State.Published });

            //a bit of test for the packages
            distributorInformation.AvailablePublishers[0].DependentComponents[0].Packages = new List<Package>();
            distributorInformation.AvailablePublishers[0].DependentComponents[0].Packages.Add(new Package { Id = Guid.NewGuid(), State = Contracts.Messages.State.Published });
            distributorInformation.AvailablePublishers[0].DependentComponents[0].Packages.Add(new Package { Id = Guid.NewGuid(), State = Contracts.Messages.State.Published });
            distributorInformation.AvailablePublishers[0].DependentComponents[0].Packages.Add(new Package { Id = Guid.NewGuid(), State = Contracts.Messages.State.Published });

            NotificationDetailsCollection = new ObservableCollection<NotificationDetailsViewModel>();
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                                                                                {
                                                                                    ParseDistributorInformation(distributorInformation);
                                                                                }));
        }

        private void ParseDistributorInformation(DistributorComponent distributorInformation)
        {
            var currentNode = new NotificationDetailsViewModel
                                  {Name = distributorInformation.Name, Type = "Distributor", State = "Online"};
            NotificationDetailsCollection.Add(currentNode);
            ParsePublishersInformation(currentNode, distributorInformation.AvailablePublishers);
        }

        private void ParsePublishersInformation(NotificationDetailsViewModel currentNode, List<AvailableComponent> availablePublishers)
        {
            currentNode.NotificationDetailsCollection=new ObservableCollection<NotificationDetailsViewModel>();
            foreach(var publisher in availablePublishers)
            {
                var currentPublisherNode = new NotificationDetailsViewModel
                                               {
                                                   Name = publisher.Name,
                                                   Type = "Publisher",
                                                   State = Enum.GetName(typeof (Status), publisher.Status)
                                               };
                currentNode.NotificationDetailsCollection.Add(currentPublisherNode);
                currentPublisherNode.NotificationDetailsCollection= new ObservableCollection<NotificationDetailsViewModel>();
                ParsePackagesInformation(currentPublisherNode.NotificationDetailsCollection, publisher.Packages);
                ParseSubscriberInformation(currentPublisherNode.NotificationDetailsCollection, publisher.DependentComponents);
            }
        }

        private void ParseSubscriberInformation(ObservableCollection<NotificationDetailsViewModel> notificationDetailsCollection, List<AvailableComponent> dependentComponents)
        {
            foreach (var subscriber in dependentComponents)
            {
                var currentSubscriberNode = new NotificationDetailsViewModel
                                                {
                                                    Name = subscriber.Name,
                                                    Type = "Subscriber",
                                                    State = Enum.GetName(typeof (Status), subscriber.Status)
                                                };
                notificationDetailsCollection.Add(currentSubscriberNode);
                currentSubscriberNode.NotificationDetailsCollection= new ObservableCollection<NotificationDetailsViewModel>();
                ParsePackagesInformation(currentSubscriberNode.NotificationDetailsCollection,subscriber.Packages);
            }
        }

        private void ParsePackagesInformation(ObservableCollection<NotificationDetailsViewModel> notificationDetailsCollection,
            List<Package> packages)
        {
            if (packages != null)
            {
                foreach (var package in packages)
                {
                    notificationDetailsCollection.Add(new NotificationDetailsViewModel
                                                          {
                                                              Name = package.Id.ToString(),
                                                              Type = "Package",
                                                              State = Enum.GetName(typeof (State), package.State)
                                                          });
                }
            }
        }
    }
}