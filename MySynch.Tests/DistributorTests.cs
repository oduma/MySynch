using System;
using System.Collections.Generic;
using System.IO;
using Moq;
using MySynch.Common;
using MySynch.Contracts;
using MySynch.Contracts.Messages;
using MySynch.Core;
using MySynch.Core.DataTypes;
using MySynch.Core.Distributor;
using MySynch.Core.Interfaces;
using MySynch.Core.Publisher;
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
            if (File.Exists("backup.xml"))
            {
                File.Copy("backup.xml", "backup1.xml", true);
                File.Delete("backup.xml");
            }

            _componentResolver=new ComponentResolver();
            _componentResolver.RegisterAll(new TestInstaller());
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists("backup1.xml"))
            {
                File.Copy("backup1.xml", "backup.xml", true);
                File.Delete("backup1.xml");
            }

        }
        [Test]
        public void BasicLoadingTestWithOneLocalChannelOneRemoteChannelAndOneMixedChannel_Ok()
        {
            Distributor distributor=new Distributor();
            distributor.InitiateDistributionMap(@"Data\distributormap.xml",_componentResolver);
            Assert.IsNotNull(distributor.AvailableChannels);
            Assert.AreEqual(1,
                            distributor.AvailableChannels.Count(
                                c =>
                                c.Status == Status.Ok && c.PublisherInfo.Port==0 &&
                                c.SubscriberInfo.Port==0));
            Assert.AreEqual(1,
                            distributor.AvailableChannels.Count(
                                c =>
                                c.Status == Status.Ok && c.PublisherInfo.Port==8765 &&
                                c.SubscriberInfo.Port==8766));
            Assert.AreEqual(1,
                            distributor.AvailableChannels.Count(
                                c =>
                                c.Status == Status.Ok && c.PublisherInfo.Port==0 &&
                                c.SubscriberInfo.Port == 8766));
            
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

        private ChangePublisher InitiateLocalPublisher()
        {
            ChangePublisher changePublisher =
                (ChangePublisher) _componentResolver.Resolve<IPublisher>("IPublisher.Local");
            var mockItemDiscoverer = MockTestHelper.MockItemDiscoverer("local source root folder");
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
            ChangePublisher changePublisher = (ChangePublisher)distributor.AvailableChannels
            .FirstOrDefault(c => c.PublisherInfo.Port==0).Publisher;
            var mockItemDiscoverer = MockTestHelper.MockItemDiscoverer("root folder");
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
            ChangePublisher changePublisher = (ChangePublisher)distributor.AvailableChannels
                .FirstOrDefault(c => c.PublisherInfo.Port==0).Publisher;
            var mockItemDiscoverer = MockTestHelper.MockItemDiscoverer("root folder");
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

        private void MockAllTheSubscribers(List<AvailableChannel> availableChannels, PublishPackageRequestResponse publishPackageRequestResponse)
        {
            foreach (var availableChannel in availableChannels)
                MockTheSubscriber(availableChannel,publishPackageRequestResponse);
        }

        private void MockTheSubscriber(AvailableChannel channel, PublishPackageRequestResponse publishPackageRequestResponse)
        {
            var mockSubscriber = new Mock<ISubscriber>();
            mockSubscriber.Setup(m => m.GetHeartbeat()).Returns(new GetHeartbeatResponse {Status = true});
            mockSubscriber.Setup(m => m.GetTargetRootFolder()).Returns(new GetTargetFolderResponse{RootFolder=@"destination root folder\Item One"});
            mockSubscriber.Setup(m => m.TryOpenChannel(null)).Returns(new TryOpenChannelResponse{Status=true});
            mockSubscriber.Setup(m => m.ApplyChangePackage(publishPackageRequestResponse)).Returns(new ApplyChangePackageResponse{Status=true});
            channel.Subscriber = mockSubscriber.Object;
        }
    }
}
