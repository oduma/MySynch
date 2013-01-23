using MySynch.Core.WCF.Clients;
using MySynch.Tests.Stubs;
using NUnit.Framework;

namespace MySynch.Tests
{
    [TestFixture]
    public class EndpointFactoryTests
    {
        [Test]
        public void GetChannel_NoChannels_Cached()
        {
            ChannelFactoryPool channelFactoryPool = ChannelFactoryPool.Instance;
            var channelFactory = channelFactoryPool.GetChannelFactory<ITest1>("test1");
            Assert.IsNotNull(channelFactory);
            Assert.AreEqual("http://localhost:8000/test1",channelFactory.Endpoint.Address.Uri.ToString());

        }

    }
}
