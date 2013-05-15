using System;
using System.IO;
using MySynch.Contracts.Messages;
using MySynch.Core.Publisher;
using NUnit.Framework;

namespace MySynch.Tests
{
    [TestFixture]
    public class PublisherPullLogicTests
    {
        [Test]
        public void GetData_Ok()
        {
            PushPublisher pushPublisher = new PushPublisher();
            var actualData = pushPublisher.GetData(new GetDataRequest { FileName = @"Data\Test\F1\F11\F12\F12.xml" });
            Assert.IsNotNull(actualData);
            FileInfo fileInfo = new FileInfo(@"Data\Test\F1\F11\F12\F12.xml");
            Assert.AreEqual(fileInfo.Length, actualData.Data.Length);

        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetData_NothingSent()
        {
            PushPublisher pushPublisher = new PushPublisher();
            Assert.IsNull(pushPublisher.GetData(null));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetData_FileNotFound()
        {
            PushPublisher pushPublisher = new PushPublisher();
            pushPublisher.GetData(new GetDataRequest { FileName = @"Data\Test\F11\F11\F12\F12.xml" });
        }
    }
}
