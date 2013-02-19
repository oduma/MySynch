using System.ServiceProcess;

namespace MySynch.WindowsService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new MySynchNodeInstance() 
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}
