using System.Collections.ObjectModel;
using System.Windows;
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
            InitializeComponent();
            IDistributorMonitorProxy distributorMonitorProxy = new DistributorMonitorClient();
            distributorMonitorProxy.InitiateUsingEndpoint("distributor");

            this.DataContext = new DistributorDetailsViewModel(distributorMonitorProxy);
        }
    }
}
