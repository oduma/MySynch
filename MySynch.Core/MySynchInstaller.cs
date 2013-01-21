using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using MySynch.Core.Interfaces;

namespace MySynch.Core
{
    public class MySynchInstaller:IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IPublisher>().ImplementedBy<ChangePublisher>().Named("IPublisher.Local"),
                               Component.For<IChangeApplyer>().ImplementedBy<ChangeApplyer>().Named("ISubScriber.Local"),
                               Component.For<ICopyStrategy>().ImplementedBy<SameSystemCopier>().Named(
                                   "ICopyStrategy.Local"));
        }
    }
}
