﻿using System;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Hardcodet.Wpf.TaskbarNotification;
using MySynch.Contracts;
using MySynch.Contracts.Messages;
using MySynch.Monitor.MVVM.ViewModels;
using MySynch.Monitor.Utils;
using MySynch.Proxies.Autogenerated.Interfaces;
using Sciendo.Common.Logging;

namespace MySynch.Monitor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application,IBrokerMonitorCallback
    {
        private TaskbarIcon tb;
        private string _brokerUrl;
        private IBrokerMonitorProxy _brokerClient;

        private void InitApplication()
        {
            //initialize NotifyIcon
            tb = (TaskbarIcon)FindResource("MyNotifyIcon");
            tb.ToolTipText = "MySynch Monitor";
            tb.ShowBalloonTip("MySynch Monitor", "Starting the monitor", BalloonIcon.Info);
            StartTheMonitor();
            ((MenuItem)tb.ContextMenu.Items[0]).Command = new RelayCommand(CloseAll);
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;


        }

        private void CloseAll()
        {
            App.Current.Shutdown();
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
            Thread.Sleep(5000);
            tb.HideBalloonTip();
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            var key = ConfigurationManager.AppSettings.AllKeys.FirstOrDefault(k => k == "BrokerUrl");
            if (key != null)
                _brokerUrl = ConfigurationManager.AppSettings[key].ToString();
            else
                _brokerUrl = string.Empty;

            InstanceContext callbackInstance= new InstanceContext(this);
            _brokerClient = new ClientHelper().ConnectToADuplexBroker(ProgressChanged, _brokerUrl,callbackInstance);
            _brokerClient.StartMonitoring();
            
        }

        protected override void OnStartup(StartupEventArgs e)
        {

            try
            {
                    InitApplication();
            }
            catch (Exception ex)
            {
                LoggingManager.LogSciendoSystemError(ex);
                InitApplication();
            }
        }

        public void NotifyNewRegistration(Registration changedRegistration)
        {

            ProgressChanged(this, new ProgressChangedEventArgs(0, RecordRegistrationAndBuildMessage(changedRegistration, true)));
        }

        private string RecordRegistrationAndBuildMessage(Registration changedRegistration, bool added)
        {
            return string.Format("{0} {1} identified by url: {2}", (added) ? "Attached" : "Detached",
                                 changedRegistration.ServiceRole, changedRegistration.ServiceUrl);
        }

        public void NotifyRemoveRegistration(Registration changedRegistration)
        {
            ProgressChanged(this, new ProgressChangedEventArgs(0, RecordRegistrationAndBuildMessage(changedRegistration,false)));
        }

        public void NotifyMessageFlow(MessageWithDestinations messageWithDestinations)
        {
            ProgressChanged(this, new ProgressChangedEventArgs(0, RecordMessageFlowAndBuildMessage(messageWithDestinations)));
        }

        private string RecordMessageFlowAndBuildMessage(MessageWithDestinations messageWithDestinations)
        {
            return string.Format("{0} of file {1} from source {2} distributed to: {3}", messageWithDestinations.OperationType, messageWithDestinations.AbsolutePath,
                                 messageWithDestinations.SourceOfMessageUrl,
                                 string.Join("\r\n",
                                             messageWithDestinations.Destinations.Select(
                                                 d =>
                                                 string.Format("{0} by subscriber:{1}",
                                                               (d.Processed) ? "processed" : "not processed", d.Url))));
        }
    }
}
