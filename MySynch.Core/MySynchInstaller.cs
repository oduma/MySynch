﻿using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using MySynch.Contracts.Messages;
using MySynch.Core.Interfaces;
using MySynch.Proxies.Autogenerated.Implementations;
using MySynch.Proxies.Autogenerated.Interfaces;
using Sciendo.Common.Logging;

namespace MySynch.Core
{
    public class MySynchInstaller:IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            LoggingManager.Debug("Installing all components.");
            using (LoggingManager.LogSciendoPerformance())
            {
                container.Register(
                    Component.For<IPublisherProxy>().ImplementedBy<PublisherClient>().Named("IPublisher.Remote"),
                    Component.For<ISubscriberProxy>().ImplementedBy<SubscriberClient>().Named("ISubscriber.Remote"),
                    Component.For<IStore<Registration>>().ImplementedBy<FileSystemStore<Registration>>().Named("IStore.Registration.FileSystemStore")
                    );
            }
        }
    }
}
