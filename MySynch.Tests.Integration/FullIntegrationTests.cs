using System.IO;
using MySynch.Contracts.Messages;
using MySynch.Proxies;
using MySynch.Proxies.Interfaces;
using NUnit.Framework;

namespace MySynch.Tests.Integration
{
    [TestFixture]
    [Category("integration")]
    [Ignore("Start the services at the specified address and after that this should run")]
    public class FullIntegrationTests
    {
        [Test]
        [Ignore(@"Requires Subscriber service to be defined on the root folder: C:\MySynch.Dest.Test.Root\ and publisher on C:\MySynch.Source.Test.Root\")]
        public void DistributorUpAndAccessible()
        {
            IDistributorMonitorProxy distributorMonitorProxy = new DistributorMonitorClient();
            distributorMonitorProxy.InitiateUsingPort(8765);
            var listAvailableChannels = distributorMonitorProxy.ListAvailableChannels();
            Assert.IsNotNull(listAvailableChannels);
            Assert.AreEqual("SCIENDO-LAPTOP", listAvailableChannels.Name);
            Assert.AreEqual(1, listAvailableChannels.Channels.Count);
            Assert.AreEqual("IPublisher.Remote", listAvailableChannels.Channels[0].PublisherInfo.InstanceName);
            Assert.AreEqual(8765, listAvailableChannels.Channels[0].PublisherInfo.Port);
            Assert.AreEqual(Status.Ok, listAvailableChannels.Channels[0].Status);
            Assert.IsNull(listAvailableChannels.Channels[0].PublisherInfo.Packages);
            Assert.IsNotNull(listAvailableChannels.Channels[0].SubscriberInfo);
            Assert.AreEqual("ISubscriber.Remote", listAvailableChannels.Channels[0].SubscriberInfo.InstanceName);
            Assert.AreEqual(8765,listAvailableChannels.Channels[0].SubscriberInfo.Port);
            Assert.AreEqual(Status.Ok, listAvailableChannels.Channels[0].SubscriberInfo.Status);
            Assert.AreEqual(@"C:\MySynch.Dest.Test.Root\", listAvailableChannels.Channels[0].SubscriberInfo.RootPath);
            Assert.IsNull(listAvailableChannels.Channels[0].SubscriberInfo.Packages);
        }

        [Test]
        public void DistributorDistreibuteMessage_Ok()
        {
            if (File.Exists(@"C:\MySynch.Dest.Test.Root\File1.xml"))
                File.Delete(@"C:\MySynch.Dest.Test.Root\File1.xml");
            IDistributorMonitorProxy distributorMonitorProxy = new DistributorMonitorClient();
            distributorMonitorProxy.InitiateUsingPort(8765);
            var listOfComponents = distributorMonitorProxy.ListAvailableChannels();
            Assert.IsNotNull(listOfComponents);
            File.Copy(@"Data\File1.xml", @"C:\MySynch.Source.Test.Root\File1.xml", true);
            
        }
    }
}
