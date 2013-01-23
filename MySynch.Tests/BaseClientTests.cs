using MySynch.Core.WCF.Clients;
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
            BaseClient<ITest1> baseClient= new BaseClient<ITest1>("test2");
            Assert.IsNotNull(baseClient);
        }

    }
}
