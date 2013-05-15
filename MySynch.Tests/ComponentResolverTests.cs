﻿using MySynch.Contracts;
using MySynch.Core;
using MySynch.Proxies.Autogenerated.Implementations;
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
            var publisher = componentResolver.Resolve<IPublisher>("IPublisher.Remote");
            Assert.IsInstanceOf(typeof(PublisherClient),publisher);
        }

        [Test]
        public void RegisterAllTwice_Ok()
        {
            MySynchComponentResolver componentResolver = new MySynchComponentResolver();
            componentResolver.RegisterAll(new MySynchInstaller());
            var publisher = componentResolver.Resolve<IPublisher>("IPublisher.Remote");
            Assert.IsInstanceOf(typeof(PublisherClient), publisher);

            MySynchComponentResolver componentResolver1 = new MySynchComponentResolver();
            componentResolver.RegisterAll(new MySynchInstaller());
            var publisher1 = componentResolver.Resolve<IPublisher>("IPublisher.Remote");
            Assert.IsInstanceOf(typeof(PublisherClient), publisher1);
            
        }
    }
}
