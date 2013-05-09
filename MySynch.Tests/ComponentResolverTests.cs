using MySynch.Common;
using MySynch.Contracts;
using MySynch.Core;
using MySynch.Core.Interfaces;
using MySynch.Core.Publisher;
using NUnit.Framework;

namespace MySynch.Tests
{
    [TestFixture]
    public class ComponentResolverTests
    {
        [Test]
        public void RegisterAllOnce_Ok()
        {
            MySynchComponentResolver componentResolver=new MySynchComponentResolver();
            componentResolver.RegisterAll(new MySynchInstaller());
            var publisher = componentResolver.Resolve<IPublisher>("IPublisher.Local");
            Assert.IsInstanceOf(typeof(ChangePublisher),publisher);
        }

        [Test]
        public void RegisterAllTwice_Ok()
        {
            MySynchComponentResolver componentResolver = new MySynchComponentResolver();
            componentResolver.RegisterAll(new MySynchInstaller());
            var publisher = componentResolver.Resolve<IPublisher>("IPublisher.Local");
            Assert.IsInstanceOf(typeof(ChangePublisher), publisher);

            MySynchComponentResolver componentResolver1 = new MySynchComponentResolver();
            componentResolver.RegisterAll(new MySynchInstaller());
            var publisher1 = componentResolver.Resolve<IPublisher>("IPublisher.Local");
            Assert.IsInstanceOf(typeof(ChangePublisher), publisher1);
            
        }
    }
}
