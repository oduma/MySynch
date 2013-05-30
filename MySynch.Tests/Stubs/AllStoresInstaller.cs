using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using MySynch.Contracts.Messages;
using MySynch.Core;
using MySynch.Core.Broker;
using MySynch.Core.Interfaces;

namespace MySynch.Tests.Stubs
{
    public class AllStoresInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IStore<Registration>>().ImplementedBy<FileSystemStore<Registration>>().Named("IStore.Registration.FileSystemStore"),
                               Component.For<IStore<Registration>>().ImplementedBy<MemoryStore<Registration>>().Named("IStore.Registration.MemoryStore"),
                               Component.For<IStore<Registration>>().ImplementedBy<DeffectiveStore<Registration>>().Named("IStore.Registration.DeffectiveStore"),
                               Component.For<IStore<Registration>>().ImplementedBy<DeffectiveGetStore<Registration>>().Named("IStore.Registration.DeffectiveGetStore"));
        }
    }
}
