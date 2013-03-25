using System;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Hardcodet.Wpf.TaskbarNotification;
using MySynch.Common.Logging;
using MySynch.Contracts.Messages;
using MySynch.Monitor.MVVM.ViewModels;
using MySynch.Monitor.Utils;
using MySynch.Proxies;
using MySynch.Proxies.Interfaces;

namespace MySynch.Monitor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon tb;
        private int _localDistributorPort;
        private IDistributorMonitorProxy _distributorMonitorProxy;
        private ListAvailableChannelsResponse _availableComponents;

        private void InitApplication()
        {
            //initialize NotifyIcon
            tb = (TaskbarIcon)FindResource("MyNotifyIcon");
            tb.ToolTipText = "MySynch Monitor";
            tb.ShowBalloonTip("MySynch Monitor", "Starting the monitor", BalloonIcon.Info);
            StartTheMonitor();
            ((MenuItem)tb.ContextMenu.Items[0]).Command=new RelayCommand(LaunchMapEditor);
            ((MenuItem)tb.ContextMenu.Items[1]).Command = new RelayCommand(LaunchMonitor);
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;


        }

        private void StartTheMonitor()
        {
            BackgroundWorker backgroundWorker= new BackgroundWorker();
            backgroundWorker.DoWork += DoWork;
            backgroundWorker.RunWorkerCompleted += RunWorkerCompleted;
            backgroundWorker.ProgressChanged += ProgressChanged;
            backgroundWorker.RunWorkerAsync();

        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            tb.HideBalloonTip();
            tb.ShowBalloonTip("Synch Monitor", e.UserState.ToString(), BalloonIcon.Info);
        }

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            tb.HideBalloonTip();
            ((MenuItem) tb.ContextMenu.Items[1]).IsEnabled = true;
            ((MenuItem) tb.ContextMenu.Items[1]).Header = "Current Activity";
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            var key = ConfigurationManager.AppSettings.AllKeys.FirstOrDefault(k => k == "LocalDistributorPort");
            if (key != null)
                _localDistributorPort = Convert.ToInt32(ConfigurationManager.AppSettings[key]);
            else
                _localDistributorPort = 0;

            new ClientHelper().ConnectToADistributor(ProgressChanged,_localDistributorPort, out _distributorMonitorProxy, out _availableComponents);
        }

        private MonitorView _monitorView;
        private void LaunchMonitor()
        {
            if (_monitorView == null)
            {
                _monitorView = new MonitorView(_localDistributorPort, _distributorMonitorProxy,_availableComponents);
                _monitorView.Closing += ViewsClosing;
            }
            _monitorView.Show();
        }

        void ViewsClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            ((Window)sender).Hide();
        }

        private MapEditorWindow _mapEditorView;

        private void LaunchMapEditor()
        {
            if(_mapEditorView==null)
            {
                _mapEditorView = new MapEditorWindow(true);
                _mapEditorView.Closing += ViewsClosing;
            }
            _mapEditorView.Show();
        }

        protected override void OnStartup(StartupEventArgs e)
        {

            try
            {
                if (Environment.GetCommandLineArgs().ShiftLeft(1).First() == "mapEditorOnly")
                {
                    var mainView = new MapEditorWindow();
                    mainView.Show();
                }
                else
                {
                    InitApplication();
                }
            }
            catch (Exception ex)
            {
                LoggingManager.LogMySynchSystemError(ex);
                InitApplication();
            }
        }
    }
}
