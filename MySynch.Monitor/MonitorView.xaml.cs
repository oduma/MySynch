using System;
using System.Windows;
using System.Windows.Controls;
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
        public MonitorView(int localDistributorPort,IDistributorMonitorProxy distributorProxy=null,ListAvailableChannelsResponse listAvailableComponentsTreeResponse=null)
        {
            InitializeComponent();
            try
            {
                var monitorViewModel = new MonitorViewModel(distributorProxy,listAvailableComponentsTreeResponse,localDistributorPort);
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
