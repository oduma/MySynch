using System;
using System.Windows;
using MySynch.Common.Logging;
using MySynch.Monitor.MVVM.ViewModels;

namespace MySynch.Monitor
{
    /// <summary>
    /// Interaction logic for MonitorView.xaml
    /// </summary>
    public partial class MonitorView : Window
    {
        public MonitorView()
        {
            InitializeComponent();
            try
            {
                var monitorViewModel = new MonitorViewModel();
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
