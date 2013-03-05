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
            var listOfComponents = distributorMonitorProxy.ListAvailableComponentsTree();
            Assert.IsNotNull(listOfComponents);
            Assert.AreEqual("SCIENDO-LAPTOP", listOfComponents.Name);
            Assert.AreEqual(1, listOfComponents.AvailablePublishers.Count);
            Assert.AreEqual("IPublisher.Remote.8765", listOfComponents.AvailablePublishers[0].Name);
            Assert.AreEqual(Status.Ok, listOfComponents.AvailablePublishers[0].Status);
            Assert.False(listOfComponents.AvailablePublishers[0].IsLocal);
            Assert.IsNull(listOfComponents.AvailablePublishers[0].Packages);
            Assert.AreEqual(1, listOfComponents.AvailablePublishers[0].DependentComponents.Count);
            Assert.AreEqual("ISubscriber.Remote.8767", listOfComponents.AvailablePublishers[0].DependentComponents[0].Name);
            Assert.AreEqual(Status.Ok, listOfComponents.AvailablePublishers[0].DependentComponents[0].Status);
            Assert.AreEqual(Status.Ok, listOfComponents.AvailablePublishers[0].DependentComponents[0].DataSourceStatus);
            Assert.False(listOfComponents.AvailablePublishers[0].DependentComponents[0].IsLocal);
            Assert.AreEqual(@"C:\MySynch.Dest.Test.Root\", listOfComponents.AvailablePublishers[0].DependentComponents[0].RootPath);
            Assert.IsNull(listOfComponents.AvailablePublishers[0].DependentComponents[0].Packages);
        }

        [Test]
        public void DistributorDistreibuteMessage_Ok()
        {
            if (File.Exists(@"C:\MySynch.Dest.Test.Root\File1.xml"))
                File.Delete(@"C:\MySynch.Dest.Test.Root\File1.xml");
            IDistributorMonitorProxy distributorMonitorProxy = new DistributorMonitorClient();
            distributorMonitorProxy.InitiateUsingPort(8765);
            var listOfComponents = distributorMonitorProxy.ListAvailableComponentsTree();
            Assert.IsNotNull(listOfComponents);
            File.Copy(@"Data\File1.xml", @"C:\MySynch.Source.Test.Root\File1.xml", true);
            
        }
    }
}
