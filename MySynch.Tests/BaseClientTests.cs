using MySynch.Tests.Stubs;
using NUnit.Framework;

namespace MySynch.Tests
{
    [TestFixture]
    public class BaseClientTests
    {
        [Test]
        public void CreateABaseClient_Ok()
        {
            ITest1Proxy baseClient= new ClientImplementation();
            Assert.IsNotNull(baseClient);
        }
    }
}
