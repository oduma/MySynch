using System;
using System.Collections.ObjectModel;
using System.Windows;
using MySynch.Common;
using MySynch.Contracts;
using MySynch.Monitor.MVVM.ViewModels;
using MySynch.Proxies;

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
                    distributorMonitorProxy.InitiateUsingEndpoint("distributor");

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
