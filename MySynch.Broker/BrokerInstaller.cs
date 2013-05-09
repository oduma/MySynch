using System.ComponentModel;
using System.ServiceProcess;


namespace MySynch.Broker
{
    [RunInstaller(true)]
    public partial class BrokerInstaller : System.Configuration.Install.Installer
    {
        private ServiceProcessInstaller process;
        private ServiceInstaller service;

        public BrokerInstaller()
        {
            process = new ServiceProcessInstaller();
            process.Account = ServiceAccount.LocalSystem;
            service = new ServiceInstaller();
#if (DEBUG)
            service.ServiceName = "MySynch.Broker.Debug";
#endif
#if (!DEBUG)
            service.ServiceName = "MySynch.Broker";
#endif
            Installers.Add(process);
            Installers.Add(service);
        }
    }
}
