using Castle.MicroKernel.Registration;
using Castle.Windsor;
using MySynch.Common;
using MySynch.Core.WCF.Clients.Discovery;

namespace MySynch.Core
{
    public class ComponentResolver
    {
        private IWindsorContainer _container;

        public void RegisterAll(IWindsorInstaller installer)
        {
            LoggingManager.Debug("Registering all with:" + installer.GetType().ToString());
            using (LoggingManager.LogMySynchPerformance())
            {
                _container = (new WindsorContainer())
                    .Install(installer);
            }
        }

        public T Resolve<T>(string name)
        {
            try
            {
                LoggingManager.Debug("Resolving for " + name);
                using (LoggingManager.LogMySynchPerformance())
                {
                    return _container.Resolve<T>(name);
                }
            }
            catch 
            {
                throw new ComponentNotRegieteredException(typeof(T).FullName,
                                                          "A component with the name: " + name + " not registered");
            }
        }

        public T Resolve<T>(string name,int port)
        {
            try
            {
                LoggingManager.Debug("Resolving for " + name + " and port " +port);
                using (LoggingManager.LogMySynchPerformance())
                {

                    T result = _container.Resolve<T>(name);
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

        public T Resolve<T, TCallBack>(TCallBack callbackInstance, string name, string endpointName) where TCallBack : class
        {
            try
            {
                LoggingManager.Debug("Resolving for " + name + " and endpoint " + endpointName);
                using (LoggingManager.LogMySynchPerformance())
                {

                    T result = _container.Resolve<T>(name);
                    ((MySynch.Core.WCF.Clients.Duplex.IInitiateClient<TCallBack>) result).InitiateUsingEndpoint(
                        callbackInstance, endpointName);
                    return result;
                }
            }
            catch
            {
                throw new ComponentNotRegieteredException(typeof(T).FullName,
                                                          "A component with the name: " + name + " not registered");
            }
        }

        public T[] ResolveAll<T>()
        {
            try
            {
                LoggingManager.Debug("Resolving all for " + typeof(T).ToString());
                using (LoggingManager.LogMySynchPerformance())
                {

                    return _container.ResolveAll<T>();
                }
            }
            catch
            {
                throw new ComponentNotRegieteredException(typeof (T).FullName, "No components of this type registered.");
            }
        }
    }
}
