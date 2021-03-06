﻿using System;
using System.Collections.ObjectModel;
using System.Windows;
using MySynch.Common;
using MySynch.Common.Logging;
using MySynch.Contracts;
using MySynch.Monitor.MVVM.ViewModels;
using MySynch.Proxies;
using MySynch.Proxies.Interfaces;

namespace MySynch.Monitor
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            using (LoggingManager.LogMySynchPerformance())
            {
                InitializeComponent();
                try
                {
                    IDistributorMonitorProxy distributorMonitorProxy = new DistributorMonitorClient();
                    distributorMonitorProxy.InitiateUsingPort(8765);

                    this.DataContext = new NotificationDetailsViewModel(distributorMonitorProxy);

                }
                catch (Exception ex)
                {
                    LoggingManager.LogMySynchSystemError(ex);
                    throw;
                }
            }
        }
    }
}
