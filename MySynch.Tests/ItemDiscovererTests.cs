﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySynch.Core;
using NUnit.Framework;

namespace MySynch.Tests
{
    [TestFixture]
    public class ItemDiscovererTests
    {
        [Test]
        public void DiscoverFromFolder_Ok()
        {
            var item = ItemDiscoverer.DiscoverFromFolder(@"Data\Test");
            Assert.IsNotNull(item);
            Assert.AreEqual("Test",item.Name);
            Assert.AreEqual(3,item.Items.Count);
            Assert.AreEqual(1,item.Items[0].Items.Count);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DiscoverFromFolder_NoFolder_Sent()
        {
            ItemDiscoverer.DiscoverFromFolder(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void DiscoverFromFolder_NoFolder_Exists()
        {
            ItemDiscoverer.DiscoverFromFolder(@"Data\Test1");
        }

    }
}
