using System;
using System.ServiceProcess;
using MySynch.Common.Logging;

namespace MySynch.Monitor.Utils
{
    internal static class ServiceHelper
    {
#if (DEBUG)
        private static string _distributorServiceName = "MySynch.Distributor.Debug";
#endif
#if (!DEBUG)
            private static string _distributorServiceName = "MySynch.Distributor";
#endif

        public static string StopDistributor()
        {
            try
            {
                ServiceController serviceController = new ServiceController(_distributorServiceName);

                serviceController.Stop();
                TimeSpan timeout = TimeSpan.FromSeconds(45);
                serviceController.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                return "Distributor Stopped.";
            }
            catch (Exception ex)
            {
                LoggingManager.LogMySynchSystemError(ex);
                return ex.Message;
            }
        }

        public static string StartDistributor()
        {
            try
            {
                ServiceController serviceController = new ServiceController(_distributorServiceName);

                serviceController.Start();
                TimeSpan timeout = TimeSpan.FromSeconds(90);
                serviceController.WaitForStatus(ServiceControllerStatus.Running, timeout);
                return "Service started.";
            }
            catch (Exception ex)
            {
                LoggingManager.LogMySynchSystemError(ex);
                return ex.Message;
            }
        }
    }
}