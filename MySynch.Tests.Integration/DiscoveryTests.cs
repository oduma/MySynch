using System.Collections.ObjectModel;
using System.ServiceModel.Discovery;
using MySynch.Contracts;
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
    }
}
