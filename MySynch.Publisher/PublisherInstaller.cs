using System.ComponentModel;
using System.ServiceProcess;


namespace MySynch.Publisher
{
    [RunInstaller(true)]
    public partial class PublisherInstaller : System.Configuration.Install.Installer
    {
        private ServiceProcessInstaller process;
        private ServiceInstaller service;

        public PublisherInstaller()
        {
            process = new ServiceProcessInstaller();
            process.Account = ServiceAccount.LocalSystem;
            service = new ServiceInstaller();
#if (DEBUG)
            service.ServiceName = "MySynch.Publisher.Debug";
#endif
#if (!DEBUG)
            service.ServiceName = "MySynch.Publisher";
#endif
            Installers.Add(process);
            Installers.Add(service);
        }
    }
}
