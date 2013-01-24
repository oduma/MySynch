using System;
using MySynch.Core;
using MySynch.Core.DataTypes;
using MySynch.Tests.Stubs;
using NUnit.Framework;
using System.Linq;

namespace MySynch.Tests
{
    [TestFixture]
    public class DistributorTests
    {
        [Test]
        public void BasicLoadinTestWithOneLocalChannelOneRemoteChannelAndOneMixedChannel_Ok()
        {
            Distributor distributor=new Distributor();
            distributor.InitiateDistributionMap(@"Data\distributormap.xml",new TestInstaller());
            Assert.IsNotNull(distributor.AvailableChannels);
            Assert.AreEqual(1,
                            distributor.AvailableChannels.Count(
                                c =>
                                c.Status == Status.Ok && string.IsNullOrEmpty(c.PublisherInfo.EndpointName) &&
                                string.IsNullOrEmpty(c.SubscriberInfo.EndpointName)));
            Assert.AreEqual(1,
                            distributor.AvailableChannels.Count(
                                c =>
                                c.Status == Status.Ok && c.PublisherInfo.EndpointName=="endpoint1" &&
                                c.SubscriberInfo.EndpointName=="endpoint2"));
            Assert.AreEqual(1,
                            distributor.AvailableChannels.Count(
                                c =>
                                c.Status == Status.Ok && string.IsNullOrEmpty(c.PublisherInfo.EndpointName) &&
                                c.SubscriberInfo.EndpointName == "endpoint1"));
            
        }

        [Test]
        public void InitiateDistributionMap_OneLocalPublisher_notWorking()
        {
            Distributor distributor = new Distributor();
            distributor.InitiateDistributionMap(@"Data\distributormap1.xml", new TestInstaller());
            Assert.IsNotNull(distributor.AvailableChannels);
            Assert.AreEqual(1, distributor.AvailableChannels.Count(c => c.Status == Status.OfflineTemporary && c.NoOfFailedAttempts==1));
        }

        [Test]
        public void InitiateDistributionMap_OneRemotePublisher_notWorking()
        {
            Distributor distributor = new Distributor();
            distributor.InitiateDistributionMap(@"Data\distributormap3.xml", new TestInstaller());
            Assert.IsNotNull(distributor.AvailableChannels);
            Assert.AreEqual(1, distributor.AvailableChannels.Count(c => c.Status == Status.OfflineTemporary && c.NoOfFailedAttempts == 1));
        }

        [Test]
        public void InitiateDistributionMap_OneLocalSubscriber_notWorking()
        {
            Distributor distributor = new Distributor();
            distributor.InitiateDistributionMap(@"Data\distributormap2.xml", new TestInstaller());
            Assert.IsNotNull(distributor.AvailableChannels);
            Assert.AreEqual(1, distributor.AvailableChannels.Count(c => c.Status == Status.OfflineTemporary && c.NoOfFailedAttempts==1));
        }

        [Test]
        public void InitiateDistributionMap_OneRemoteSubscriber_notWorking()
        {
            Distributor distributor = new Distributor();
            distributor.InitiateDistributionMap(@"Data\distributormap4.xml", new TestInstaller());
            Assert.IsNotNull(distributor.AvailableChannels);
            Assert.AreEqual(1, distributor.AvailableChannels.Count(c => c.Status == Status.OfflineTemporary && c.NoOfFailedAttempts == 1));
        }

        [Test]
        public void ChannelGoesDownPermanentlyOnlyOneChanelLocal_Ok()
        {
            Distributor distributor = new Distributor();
            distributor.InitiateDistributionMap(@"Data\distributormap2.xml", new TestInstaller());
            Assert.IsNotNull(distributor.AvailableChannels);
            Assert.AreEqual(1, distributor.AvailableChannels.Count(c => c.Status == Status.OfflineTemporary && c.NoOfFailedAttempts == 1));
            distributor.DistributeMessages();
            Assert.AreEqual(1, distributor.AvailableChannels.Count(c => c.Status == Status.OfflineTemporary && c.NoOfFailedAttempts == 2));
            distributor.DistributeMessages();
            Assert.AreEqual(1, distributor.AvailableChannels.Count(c => c.Status == Status.OfflineTemporary && c.NoOfFailedAttempts == 3));
            distributor.DistributeMessages();
            Assert.AreEqual(1, distributor.AvailableChannels.Count(c => c.Status == Status.OfflinePermanent && c.NoOfFailedAttempts == 3));
            
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void LoadDistributorWrongMapFile()
        {
            Distributor distributor = new Distributor();
            distributor.InitiateDistributionMap(@"Data\nonmap.xml", new TestInstaller());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LoadDistributorNoMapFile()
        {
            Distributor distributor = new Distributor();
            distributor.InitiateDistributionMap(null, new TestInstaller());
        }
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LoadDistributorNoInstaller()
        {
            Distributor distributor = new Distributor();
            distributor.InitiateDistributionMap(@"Data\distributormap.xml", null);
        }
    }
}
