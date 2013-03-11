using System;
using System.Windows;
using MySynch.Common.Logging;
using MySynch.Monitor.MVVM.ViewModels;

namespace MySynch.Monitor
{
    /// <summary>
    /// Interaction logic for MapEditorWindow.xaml
    /// </summary>
    public partial class MapEditorWindow : Window
    {
        public MapEditorWindow()
        {
            using (LoggingManager.LogMySynchPerformance())
            {
                InitializeComponent();
                try
                {

                    this.DataContext = new MapEditorViewModel();

                }
                catch (Exception ex)
                {
                    LoggingManager.LogMySynchSystemError(ex);
                    throw;
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
