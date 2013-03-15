using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Hardcodet.Wpf.TaskbarNotification;
using MySynch.Common.Logging;
using MySynch.Monitor.MVVM.ViewModels;
using MySynch.Monitor.Utils;

namespace MySynch.Monitor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon tb;

        private void InitApplication()
        {
            //initialize NotifyIcon
            tb = (TaskbarIcon)FindResource("MyNotifyIcon");
            ((MenuItem)tb.ContextMenu.Items[0]).Command=new RelayCommand(LaunchMapEditor);

        }

        private MapEditorWindow _mapEditorView; 
        private void LaunchMapEditor()
        {
            if(_mapEditorView==null)
                _mapEditorView = new MapEditorWindow(true);
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
