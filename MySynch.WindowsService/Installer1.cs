using System.ComponentModel;
using System.ServiceProcess;


namespace MySynch.WindowsService
{
    [RunInstaller(true)]
    public partial class Installer1 : System.Configuration.Install.Installer
    {
        private ServiceProcessInstaller process;
        private ServiceInstaller service;

    public Installer1()
    {
        InitializeComponent();

        process = new ServiceProcessInstaller();
        process.Account = ServiceAccount.LocalSystem;
        service = new ServiceInstaller();
        service.ServiceName = "MySynch.PeerNode";
        Installers.Add(process);
        Installers.Add(service);
    }
    }
}
