using System;
using MySynch.Common.IOC;
using MySynch.Common.Logging;
using MySynch.Core.WCF.Clients.Discovery;

namespace MySynch.Core
{
    public class MySynchComponentResolver:ComponentResolver
    {
        public void UnRegister<T>(string name, int port)
        {
            try
            {
                LoggingManager.Debug("Unregistering for " + name);
                
                using(LoggingManager.LogMySynchPerformance())
                {
                    T component = Resolve<T>(name, port);
                    Container.Release(component);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public T Resolve<T>(string name,int port)
        {
            try
            {
                LoggingManager.Debug("Resolving for " + name + " and port " +port);
                using (LoggingManager.LogMySynchPerformance())
                {

                    T result = Container.Resolve<T>(name);
                    ((IInitiateClient) result).InitiateUsingPort(port);
                    return result;
                }
            }
            catch
            {
                throw new ComponentNotRegieteredException(typeof (T).FullName,
                                                          "A component with the name: " + name + " not registered");
            }
        }

        public T Resolve<T, TCallBack>(TCallBack callbackInstance, string name, int port) where TCallBack : class
        {
            try
            {
                LoggingManager.Debug("Resolving for " + name + " and port " + port);
                using (LoggingManager.LogMySynchPerformance())
                {

                    T result = Container.Resolve<T>(name);
                    ((IInitiateClient) result).InitiateDuplexUsingPort(
                        callbackInstance, port);
                    return result;
                }
            }
            catch
            {
                throw new ComponentNotRegieteredException(typeof(T).FullName,
                                                          "A component with the name: " + name + " not registered");
            }
        }
    }
}
