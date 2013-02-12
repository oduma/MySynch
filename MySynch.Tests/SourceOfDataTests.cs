using System;
using System.IO;
using MySynch.Contracts.Messages;
using MySynch.Core;
using NUnit.Framework;

namespace MySynch.Tests
{
    [TestFixture]
    public class SourceOfDataTests
    {
        [Test]
        public void LocalSourceOfData_GetData_Ok()
        {
            LocalSourceOfData localSourceOfData= new LocalSourceOfData();
            Assert.IsNull(localSourceOfData.GetData(null));
        }

        [Test]
        public void RemoteSourceOfData_GetData_Ok()
        {
            RemoteSourceOfData remoteSourceOfData = new RemoteSourceOfData();
            var actualData = remoteSourceOfData.GetData(new RemoteRequest { FileName = @"Data\Test\F1\F11\F12\F12.xml" });
            Assert.IsNotNull(actualData);
            FileInfo fileInfo = new FileInfo(@"Data\Test\F1\F11\F12\F12.xml");
            Assert.AreEqual(fileInfo.Length,actualData.Data.Length);
            
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoteSourceOfData_NothingSent()
        {
            RemoteSourceOfData remoteSourceOfData = new RemoteSourceOfData();
            remoteSourceOfData.GetData(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void RemoteSourceOfData_FileNotFound()
        {
            RemoteSourceOfData remoteSourceOfData = new RemoteSourceOfData();
            remoteSourceOfData.GetData(new RemoteRequest { FileName = @"Data\Test\F11\F11\F12\F12.xml" });
        }
    }
}
