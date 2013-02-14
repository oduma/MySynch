using System.ComponentModel;
using System.ServiceProcess;


namespace MySynch.Distributor
{
    [RunInstaller(true)]
    public partial class DistributorInstaller : System.Configuration.Install.Installer
    {
        private ServiceProcessInstaller process;
        private ServiceInstaller service;

        public DistributorInstaller()
        {
            process = new ServiceProcessInstaller();
            process.Account = ServiceAccount.LocalSystem;
            service = new ServiceInstaller();
            service.ServiceName = "MySynch.Distributor";
            Installers.Add(process);
            Installers.Add(service);
        }
    }
}
