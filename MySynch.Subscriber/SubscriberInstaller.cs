using System.ComponentModel;
using System.ServiceProcess;


namespace MySynch.Subscriber
{
    [RunInstaller(true)]
    public partial class SubscriberInstaller : System.Configuration.Install.Installer
    {
        private ServiceProcessInstaller process;
        private ServiceInstaller service;

        public SubscriberInstaller()
        {
            process = new ServiceProcessInstaller();
            process.Account = ServiceAccount.LocalSystem;
            service = new ServiceInstaller();
            service.ServiceName = "MySynch.Subscriber";
            Installers.Add(process);
            Installers.Add(service);
        }
    }
}
