﻿using System;
using System.Windows;
using MySynch.Common.Logging;
using MySynch.Contracts.Messages;
using MySynch.Monitor.MVVM.ViewModels;
using MySynch.Proxies.Interfaces;

namespace MySynch.Monitor
{
    /// <summary>
    /// Interaction logic for MonitorView.xaml
    /// </summary>
    public partial class MonitorView : Window
    {
        public MonitorView(IDistributorMonitorProxy distributorProxy=null,ListAvailableChannelsResponse listAvailableComponentsTreeResponse=null)
        {
            InitializeComponent();
            try
            {
                var monitorViewModel = new MonitorViewModel(distributorProxy,listAvailableComponentsTreeResponse);
                monitorViewModel.InitiateView();
                this.DataContext = monitorViewModel;

            }
            catch (Exception ex)
            {
                LoggingManager.LogMySynchSystemError(ex);
                throw;
            }
        }
    }
}
