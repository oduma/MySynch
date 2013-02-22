using System;
using System.Collections.Generic;
using Moq;
using MySynch.Contracts;
using MySynch.Contracts.Messages;
using MySynch.Core;
using MySynch.Core.DataTypes;
using MySynch.Core.Interfaces;
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
        public void LoadDistributorNoRegisteredPublishers()
        {
            Distributor distributor = new Distributor();
            distributor.InitiateDistributionMap(@"Data\distributormap.xml", new ComponentResolver());
            Assert.AreEqual(0, distributor.AvailableChannels.Count(c => c.Status == Status.Ok));
        }
        [Test]
        public void DistributeMessages_OneChannel_Ok()
        {

            Distributor distributor = new Distributor();
            distributor.InitiateDistributionMap(@"Data\distributormaplocal2local.xml", _componentResolver);
            ChangePublisher changePublisher = InitiateLocalPublisher();
            changePublisher.PublishPackage();
            MockAllTheSubscribers(distributor.AvailableChannels,changePublisher.PublishedPackageNotDistributed[0]);
            distributor.DistributeMessages();
            Assert.AreEqual(0, changePublisher.PublishedPackageNotDistributed.Count);
        }
        private IItemDiscoverer MockItemDiscoverer(string folderPath)
        {
            var mockItemDiscoverer = new Mock<IItemDiscoverer>();
            mockItemDiscoverer.Setup(m => m.DiscoverFromFolder(folderPath)).Returns(new SynchItem());
            return mockItemDiscoverer.Object;
        }

        private ChangePublisher InitiateLocalPublisher()
        {
            ChangePublisher changePublisher =
                (ChangePublisher) _componentResolver.Resolve<IPublisher>("IPublisher.Local");
            var mockItemDiscoverer = MockItemDiscoverer("local source root folder");
            changePublisher.Initialize("local source root folder",mockItemDiscoverer);
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
            changePublisher.PublishPackage();
            MockAllTheSubscribers(distributor.AvailableChannels,changePublisher.PublishedPackageNotDistributed[0]);
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

        [Test]
        public void ListAllComponents_Ok()
        {
            Distributor distributor = new Distributor();
            distributor.InitiateDistributionMap(@"Data\distributormap.xml", _componentResolver);
            var compo = distributor.ListAvailableComponentsTree();
            Assert.IsNotNull(compo);
            Assert.AreEqual(2,compo.AvailablePublishers.Count);
            Assert.IsNotNull(compo.AvailablePublishers[0].DependentComponents);
            Assert.IsNotNull(compo.AvailablePublishers[1].DependentComponents);
            Assert.AreEqual(2,compo.AvailablePublishers[0].DependentComponents.Count);
            foreach(var c1 in compo.AvailablePublishers[0].DependentComponents)
                Assert.AreEqual(Status.Ok,c1.Status);
            Assert.AreEqual(1, compo.AvailablePublishers[1].DependentComponents.Count);

        }

        [Test]
        public void ListAllComponents_AndRegisterMessages_Ok()
        {
            Distributor distributor = new Distributor();
            distributor.InitiateDistributionMap(@"Data\distributormap5.xml", _componentResolver);
            ChangePublisher changePublisher = (ChangePublisher)distributor.AvailableChannels.FirstOrDefault(c => string.IsNullOrEmpty(c.PublisherInfo.EndpointName)).PublisherInfo.Publisher;
            var mockItemDiscoverer = MockItemDiscoverer("root folder");
            changePublisher.Initialize("root folder",mockItemDiscoverer);
            changePublisher.QueueInsert(@"root folder\Item One");
            changePublisher.QueueInsert(@"root folder\ItemTwo");
            changePublisher.PublishPackage();
            MockAllTheSubscribers(distributor.AvailableChannels,changePublisher.PublishedPackageNotDistributed[0]);
            distributor.DistributeMessages();
            var compo = distributor.ListAvailableComponentsTree();
            Assert.IsNotNull(compo);
            Assert.AreEqual(1, compo.AvailablePublishers.Count);
            Assert.IsNotNull(compo.AvailablePublishers[0].DependentComponents);
            Assert.AreEqual(1, compo.AvailablePublishers[0].DependentComponents.Count);
            Assert.AreEqual(1,compo.AvailablePublishers[0].Packages.Count(p=>p.State==State.Removed));
            Assert.AreEqual(1, compo.AvailablePublishers[0].Packages[0].PackageMessages.Count(m => m.AbsolutePath == @"root folder\Item One"));
            Assert.AreEqual(2, compo.AvailablePublishers[0].Packages[0].PackageMessages.Count(m => m.OperationType==OperationType.Insert));
            Assert.AreEqual(1, compo.AvailablePublishers[0].DependentComponents[0].Packages.Count(p => p.State == State.Removed));
            //Assert.AreEqual(1, compo.AvailablePublishers[0].DependentComponents[0].Packages[0].PackageMessages.Count(m => m.AbsolutePath == @"destination root folder\Item One"));
            //Assert.AreEqual(2, compo.AvailablePublishers[0].DependentComponents[0].Packages[0].PackageMessages.Count(m => m.OperationType==OperationType.Insert));

        }

        [Test]
        public void ListAllComponents_RegisterAndUnRegisterMessages_Ok()
        {
            Distributor distributor = new Distributor();
            distributor.InitiateDistributionMap(@"Data\distributormap5.xml", _componentResolver);
            ChangePublisher changePublisher = (ChangePublisher)distributor.AvailableChannels.FirstOrDefault(c => string.IsNullOrEmpty(c.PublisherInfo.EndpointName)).PublisherInfo.Publisher;
            var mockItemDiscoverer = MockItemDiscoverer("root folder");
            changePublisher.Initialize("root folder", mockItemDiscoverer);
            changePublisher.QueueInsert(@"root folder\Item One");
            changePublisher.QueueInsert(@"root folder\ItemTwo");
            changePublisher.PublishPackage();
            MockAllTheSubscribers(distributor.AvailableChannels,changePublisher.PublishedPackageNotDistributed[0]);
            distributor.DistributeMessages();
            var compo = distributor.ListAvailableComponentsTree();
            distributor.DistributeMessages();
            Assert.IsNotNull(compo);
            Assert.AreEqual(1, compo.AvailablePublishers.Count);
            Assert.IsNotNull(compo.AvailablePublishers[0].DependentComponents);
            Assert.AreEqual(1, compo.AvailablePublishers[0].DependentComponents.Count);
            Assert.AreEqual(0, compo.AvailablePublishers[0].Packages.Count);
            Assert.AreEqual(0, compo.AvailablePublishers[0].DependentComponents[0].Packages.Count);
        }

        private void MockAllTheSubscribers(List<AvailableChannel> availableChannels, ChangePushPackage changePushPackage)
        {
            foreach (var availableChannel in availableChannels)
                MockTheSubscriber(availableChannel,changePushPackage);
        }

        private void MockTheSubscriber(AvailableChannel channel, ChangePushPackage changePushPackage)
        {
            var mockSubscriber = new Mock<ISubscriber>();
            mockSubscriber.Setup(m => m.GetHeartbeat()).Returns(new HeartbeatResponse {Status = true});
            mockSubscriber.Setup(m => m.GetTargetRootFolder()).Returns(@"destination root folder\Item One");
            mockSubscriber.Setup(m => m.TryOpenChannel(null)).Returns(true);
            mockSubscriber.Setup(m => m.ApplyChangePackage(changePushPackage)).Returns(true);
            channel.SubscriberInfo.Subscriber = mockSubscriber.Object;
        }

        [Test]
        public void InitialSynchronization_NoNeed()
        {
            Assert.Fail();
        }

        [Test]
        public void InitialSynchronization_OneChannel_Ok()
        {
            Assert.Fail();
        }

    }
}
