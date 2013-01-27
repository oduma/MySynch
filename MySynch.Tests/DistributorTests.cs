using System;
using MySynch.Contracts;
using MySynch.Core;
using MySynch.Core.DataTypes;
using MySynch.Core.Interfaces;
using MySynch.Proxies;
using MySynch.Tests.Stubs;
using NUnit.Framework;
using System.Linq;

namespace MySynch.Tests
{
    [TestFixture]
    public class DistributorTests
    {
        private ComponentResolver _componentResolver;

        [SetUp]
        public void SetUp()
        {
            _componentResolver=new ComponentResolver();
            _componentResolver.RegisterAll(new TestInstaller());
        }
        [Test]
        public void BasicLoadinTestWithOneLocalChannelOneRemoteChannelAndOneMixedChannel_Ok()
        {
            Distributor distributor=new Distributor();
            distributor.InitiateDistributionMap(@"Data\distributormap.xml",_componentResolver);
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
            distributor.InitiateDistributionMap(@"Data\distributormap1.xml", _componentResolver);
            Assert.IsNotNull(distributor.AvailableChannels);
            Assert.AreEqual(1, distributor.AvailableChannels.Count(c => c.Status == Status.OfflineTemporary && c.NoOfFailedAttempts==1));
        }

        [Test]
        public void InitiateDistributionMap_OneRemotePublisher_notWorking()
        {
            Distributor distributor = new Distributor();
            distributor.InitiateDistributionMap(@"Data\distributormap3.xml", _componentResolver);
            Assert.IsNotNull(distributor.AvailableChannels);
            Assert.AreEqual(1, distributor.AvailableChannels.Count(c => c.Status == Status.OfflineTemporary && c.NoOfFailedAttempts == 1));
        }

        [Test]
        public void InitiateDistributionMap_OneLocalSubscriber_notWorking()
        {
            Distributor distributor = new Distributor();
            distributor.InitiateDistributionMap(@"Data\distributormap2.xml", _componentResolver);
            Assert.IsNotNull(distributor.AvailableChannels);
            Assert.AreEqual(1, distributor.AvailableChannels.Count(c => c.Status == Status.OfflineTemporary && c.NoOfFailedAttempts==1));
        }

        [Test]
        public void InitiateDistributionMap_OneRemoteSubscriber_notWorking()
        {
            Distributor distributor = new Distributor();
            distributor.InitiateDistributionMap(@"Data\distributormap4.xml", _componentResolver);
            Assert.IsNotNull(distributor.AvailableChannels);
            Assert.AreEqual(1, distributor.AvailableChannels.Count(c => c.Status == Status.OfflineTemporary && c.NoOfFailedAttempts == 1));
        }

        [Test]
        public void ChannelGoesDownPermanentlyOnlyOneChanelLocal_Ok()
        {
            Distributor distributor = new Distributor();
            distributor.InitiateDistributionMap(@"Data\distributormap2.xml", _componentResolver);
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
            distributor.InitiateDistributionMap(@"Data\nonmap.xml", _componentResolver);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LoadDistributorNoMapFile()
        {
            Distributor distributor = new Distributor();
            distributor.InitiateDistributionMap(null, _componentResolver);
        }
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LoadDistributorNoResolver()
        {
            Distributor distributor = new Distributor();
            distributor.InitiateDistributionMap(@"Data\distributormap.xml", null);
        }

        [Test]
        [ExpectedException(typeof(ComponentNotRegieteredException))]
        public void LoadDistributorNoRegisteredPublishers()
        {
            Distributor distributor = new Distributor();
            distributor.InitiateDistributionMap(@"Data\distributormap.xml", new ComponentResolver());
            
        }
        [Test]
        public void DistributeMessages_OneChannel_Ok()
        {

            Distributor distributor = new Distributor();
            distributor.InitiateDistributionMap(@"Data\distributormaplocal2local.xml", _componentResolver);
            
            ChangePublisher changePublisher = InitiateLocalPublisher();
            distributor.DistributeMessages();
            Assert.AreEqual(0, changePublisher.PublishedPackageNotDistributed.Count);
        }

        private ChangePublisher InitiateLocalPublisher()
        {
            ChangePublisher changePublisher =
                (ChangePublisher) _componentResolver.Resolve<IPublisher>("IPublisher.Local");
            changePublisher.Initialize("local source root folder");
            changePublisher.QueueInsert("local source root folder\\item one");
            changePublisher.QueueInsert("local source root folder\\item two");
            return changePublisher;
        }

        [Test]
        public void DsitributeMessages_OnePublisherTwoChannels_BothOk()
        {
            Distributor distributor = new Distributor();
            distributor.InitiateDistributionMap(@"Data\distributormaplocal2localandremote.xml", _componentResolver);

            ChangePublisher changePublisher = InitiateLocalPublisher();
            distributor.DistributeMessages();
            Assert.AreEqual(0, changePublisher.PublishedPackageNotDistributed.Count);
        }

        [Test]
        public void DsitributeMessages_OnepublisherThreeChannels_OneNotPresentOneFailing()
        {
            Distributor distributor = new Distributor();
            distributor.InitiateDistributionMap(@"Data\distributormaplocal2localandremoteNP.xml", _componentResolver);

            ChangePublisher changePublisher = InitiateLocalPublisher();
            distributor.DistributeMessages();
            Assert.AreEqual(1, changePublisher.PublishedPackageNotDistributed.Count);
            Assert.AreEqual(2, changePublisher.PublishedPackageNotDistributed[0].ChangePushItems.Count);
        }

    }
}
