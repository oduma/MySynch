using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySynch.Core;
using MySynch.Core.DataTypes;
using NUnit.Framework;

namespace MySynch.Tests
{
    [TestFixture]
    public class ItemDiscovererTests
    {
        [Test]
        public void DiscoverFromFolder_Ok()
        {
            var itemDiscoverer = new ItemDiscoverer(@"Data\Test");
            itemDiscoverer.DiscoveringFolder += new EventHandler<Core.DataTypes.FolderDiscoveredArg>(itemDiscoverer_DiscoveringFolder);
            var item = itemDiscoverer.DiscoverFromFolder(@"Data\Test");
            Assert.IsNotNull(item);
            Assert.AreEqual("Test",item.Name);
            Assert.AreEqual(3,item.Items.Count);
            Assert.AreEqual(1,item.Items[0].Items.Count);
        }

        void itemDiscoverer_DiscoveringFolder(object sender, FolderDiscoveredArg e)
        {
            Assert.IsNotNullOrEmpty(e.Folder);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DiscoverFromFolder_NoFolder_Sent()
        {
            var itemDiscoverer = new ItemDiscoverer(@"Data\Test");
            var item = itemDiscoverer.DiscoverFromFolder(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void DiscoverFromFolder_NoFolder_Exists()
        {
            var itemDiscoverer = new ItemDiscoverer(@"Data\Test1");
            var item = itemDiscoverer.DiscoverFromFolder(@"Data\Test1");
        }

    }
}
