using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using MySynch.Core.WCF.Clients;

namespace MySynch.Core
{
    public class ComponentResolver
    {
        private IWindsorContainer _container;

        public void RegisterAll(IWindsorInstaller installer)
        {
            _container=(new WindsorContainer())
                .Install(installer);
        }
        public T Resolve<T>(string name)
        {
            try
            {
                return _container.Resolve<T>(name);
            }
            catch 
            {
                throw new NotImplementedException();
            }
        }
        public T Resolve<T>(string name,string endpointName)
        {
            try
            {
                T result = _container.Resolve<T>(name);
                ((IInitiateClient)result).InitiateUsingEndpoint(endpointName);
                return result;
            }
            catch
            {
                throw new NotImplementedException();
            }
        }

    }
}
