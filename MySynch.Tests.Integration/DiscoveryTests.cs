using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel.Discovery;
using MySynch.Contracts;
using MySynch.Core.WCF.Clients.Discovery;
using NUnit.Framework;

namespace MySynch.Tests.Integration
{
    [TestFixture]
    [Category("integration")]
    public class DiscoveryTests
    {
        [Test]
        [Ignore("The service with discovery has to be running")]
        public void Find_Service_Publisher_Ok()
        {
            DiscoveryClient discoveryClient =
                new DiscoveryClient(new UdpDiscoveryEndpoint());

            var publisherServices =
                discoveryClient.Find(new FindCriteria(typeof(IPublisher)));

            discoveryClient.Close();

            Assert.False(publisherServices.Endpoints.Count == 0);
            var serviceAddress = publisherServices.Endpoints[0].Address;
            Assert.IsNotNull(serviceAddress);
        }

        [Test]
        [Ignore("Two services with discovery has to be running")]
        public void Find_Services_All_Distributors_Ok()
        {
            Assert.AreEqual(2, DiscoveryHelper.FindServices<IDistributorMonitor>().Count());
        }

        [Test]
        [Ignore("No services with discovery has to be running")]
        public void FindServices_All_Something_NothingFound()
        {
            Assert.IsNull(DiscoveryHelper.FindServices<IDistributorMonitor>());
        }
    }
}
