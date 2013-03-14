using System;
using System.Windows;
using MySynch.Common.Logging;

namespace MySynch.Monitor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //private TaskbarIcon tb;

        private void InitApplication()
        {
            //initialize NotifyIcon
            //tb = (TaskbarIcon)FindResource("MyNotifyIcon");
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                MessageBox.Show(Environment.GetCommandLineArgs()[1]);
                var mainView = new MapEditorWindow();
                mainView.Show();
            }
            catch (Exception ex)
            {
                LoggingManager.LogMySynchSystemError(ex);
            }
        }
    }
}
