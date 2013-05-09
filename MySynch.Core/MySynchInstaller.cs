using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using MySynch.Common.Logging;
using MySynch.Common.Serialization;
using MySynch.Contracts;
using MySynch.Contracts.Messages;
using MySynch.Core.Publisher;
using MySynch.Proxies;
using MySynch.Proxies.Interfaces;

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
                    Component.For<ISubscriber>().ImplementedBy<Subscriber.Subscriber>().Named("ISubScriber.Local"),
                    Component.For<ISourceOfData>().ImplementedBy<LocalSourceOfData>().Named(
                        "ISourceOfData.Local"),
                    Component.For<IPublisherProxy>().ImplementedBy<PublisherClient>().Named("IPublisher.Remote"),
                    Component.For<ISubscriberProxy>().ImplementedBy<SubscriberClient>().Named("ISubscriber.Remote"),
                    Component.For<ISourceOfDataProxy>().ImplementedBy<SourceOfDataClient>().Named(
                        "ISourceOfData.Remote"),
                    Component.For<IStore<Registration>>().ImplementedBy<FileSystemStore<Registration>>().Named("IStore.Registration.FileSystemStore")
                    );
            }
        }
    }
}
