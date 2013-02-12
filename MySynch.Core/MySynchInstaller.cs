using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using MySynch.Common;
using MySynch.Contracts;
using MySynch.Core.Interfaces;
using MySynch.Proxies;

namespace MySynch.Core
{
    public class MySynchInstaller:IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            LoggingManager.Debug("Installing all components.");
            using (LoggingManager.LogMySynchPerformance())
            {
                container.Register(
                    Component.For<IPublisher>().ImplementedBy<ChangePublisher>().Named("IPublisher.Local"),
                    Component.For<IChangeApplyer>().ImplementedBy<ChangeApplyer>().Named("ISubScriber.Local"),
                    Component.For<ICopyStrategy>().ImplementedBy<SameSystemCopier>().Named(
                        "ICopyStrategy.Local"),
                    Component.For<ISourceOfData>().ImplementedBy<LocalSourceOfData>().Named(
                        "ISourceOfData.Local"),
                    Component.For<IPublisherProxy>().ImplementedBy<PublisherClient>().Named("IPublisher.Remote"),
                    Component.For<ISubscriberProxy>().ImplementedBy<SubscriberClient>().Named("ISubscriber.Remote"),
                    Component.For<ICopyStrategy>().ImplementedBy<RemoteSystemCopier>().Named(
                        "ICopyStrategy.Remote"),
                    Component.For<ISourceOfDataProxy>().ImplementedBy<SourceOfDataClient>().Named(
                        "ISourceOfData.Remote")
                    );
            }
        }
    }
}
