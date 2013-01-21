using MySynch.Core;
using MySynch.Core.Interfaces;
using NUnit.Framework;

namespace MySynch.Tests
{
    [TestFixture]
    public class ComponentResolverTests
    {
        [Test]
        public void RegisterAllOnce_Ok()
        {
            ComponentResolver componentResolver=new ComponentResolver();
            componentResolver.RegisterAll(new MySynchInstaller());
            var publisher = componentResolver.Resolve<IPublisher>("IPublisher.Local");
            Assert.IsInstanceOf(typeof(ChangePublisher),publisher);
        }

        [Test]
        public void RegisterAllTwice_Ok()
        {
            ComponentResolver componentResolver = new ComponentResolver();
            componentResolver.RegisterAll(new MySynchInstaller());
            var publisher = componentResolver.Resolve<IPublisher>("IPublisher.Local");
            Assert.IsInstanceOf(typeof(ChangePublisher), publisher);

            ComponentResolver componentResolver1 = new ComponentResolver();
            componentResolver.RegisterAll(new MySynchInstaller());
            var publisher1 = componentResolver.Resolve<IPublisher>("IPublisher.Local");
            Assert.IsInstanceOf(typeof(ChangePublisher), publisher1);
            
        }
    }
}
