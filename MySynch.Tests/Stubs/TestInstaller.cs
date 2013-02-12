using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using MySynch.Contracts;
using MySynch.Core;
using MySynch.Core.Interfaces;
using MySynch.Proxies;

namespace MySynch.Tests.Stubs
{
    public class TestInstaller:IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IPublisher>().ImplementedBy<ChangePublisher>().Named("IPublisher.Local"),
                               Component.For<IChangeApplyer>().ImplementedBy<ChangeApplyer>().Named("ISubScriber.Local"),
                               Component.For<ISourceOfData>().ImplementedBy<LocalSourceOfData>().Named(
                                   "ISourceOfData.Local"),
                               Component.For<IPublisher>().ImplementedBy<ChangePublisherNotPresent>().Named("IPublisher.NotPresent"),
                               Component.For<IChangeApplyer>().ImplementedBy<ChangeApplyerNotPresent>().Named("ISubScriber.NotPresent"),
                               Component.For<IPublisherProxy>().ImplementedBy<MockRemotePublisher>().Named("IPublisher.Remote"),
                               Component.For<ISubscriberProxy>().ImplementedBy<MockRemoteSubscriber>().Named("ISubScriber.Remote"),
                               Component.For<IPublisherProxy>().ImplementedBy<MockRemotePublisherNotPresent>().Named("IPublisher.Remote.NotPresent"),
                               Component.For<ISubscriberProxy>().ImplementedBy<MockRemoteSubscriberNotPresent>().Named("ISubScriber.Remote.NotPresent"));
        }
    }
}
