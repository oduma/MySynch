using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

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
    }
}
