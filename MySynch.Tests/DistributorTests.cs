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
        public void BasicLoadinTestWithOneLocalChannel_Ok()
        {
            Distributor distributor=new Distributor();
            distributor.InitiateDistributionMap(@"Data\distributormap.xml",new TestInstaller());
            Assert.IsNotNull(distributor.AvailableChannels);
            Assert.AreEqual(1,distributor.AvailableChannels.Count(c=>c.Status==Status.Ok));
        }

        [Test]
        public void InitiateDistributionMap_OnePublisher_notWorking()
        {
            Distributor distributor = new Distributor();
            distributor.InitiateDistributionMap(@"Data\distributormap1.xml", new TestInstaller());
            Assert.IsNotNull(distributor.AvailableChannels);
            Assert.AreEqual(1, distributor.AvailableChannels.Count(c => c.Status == Status.OfflineTemporary && c.NoOfFailedAttempts==1));
            
        }

        [Test]
        public void InitiateDistributionMap_OneSubscriber_notWorking()
        {
            Distributor distributor = new Distributor();
            distributor.InitiateDistributionMap(@"Data\distributormap2.xml", new TestInstaller());
            Assert.IsNotNull(distributor.AvailableChannels);
            Assert.AreEqual(1, distributor.AvailableChannels.Count(c => c.Status == Status.OfflineTemporary && c.NoOfFailedAttempts==1));
        }

        [Test]
        public void ChannelGoesDownPermanently_Ok()
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

    }
}
