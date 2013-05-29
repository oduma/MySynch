﻿using System;
using MySynch.Contracts.Messages;
using MySynch.Core.Configuration;
using MySynch.Core.DataTypes;
using MySynch.Core.Publisher;
using MySynch.Proxies.Autogenerated.Implementations;
using NUnit.Framework;

namespace MySynch.Tests
{
    [TestFixture]
    public class PublisherPushLogicTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Initialize_NoBroker()
        {
            PushPublisher pushPublisher = new PushPublisher();
            pushPublisher.Initialize(null,null,null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Initialize_NoRootFolder()
        {
            PushPublisher pushPublisher = new PushPublisher();
            pushPublisher.Initialize(new BrokerClient(),new MySynchPublisherConfigurationSection(),null);
        }
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Initialize_NoHostUrl()
        {
            PushPublisher pushPublisher = new PushPublisher();
            pushPublisher.Initialize(new BrokerClient(), new MySynchPublisherConfigurationSection { LocalRootFolder = "my root folder" }, string.Empty);
        }

        [Test]
        public void Initialize_Ok()
        {
            PushPublisher pushPublisher = new PushPublisher();
            pushPublisher.Initialize(new BrokerClient(), new MySynchPublisherConfigurationSection { LocalRootFolder = @"c:\" }, "my host url");
            Assert.AreEqual("my host url", pushPublisher.HostUrl);
        }

        [Test]
        public void UpdateCurrentRepository_Insert_Ok()
        {
            PushPublisher pushPublisher = new PushPublisher();
            pushPublisher.Initialize(new BrokerClient(),new MySynchPublisherConfigurationSection{LocalRootFolder=@"Data"},"my host url");
            Assert.True(pushPublisher.UpdateCurrentRepository(@"Data\Test\F1\F11\F12\F12.xml", OperationType.Insert));
            Assert.IsNotNull(pushPublisher.CurrentRepository);
            Assert.AreEqual(1,pushPublisher.CurrentRepository.Items.Count);
            Assert.AreEqual(@"Data", pushPublisher.CurrentRepository.SynchItemData.Identifier);
            Assert.AreEqual(@"Data\Test", pushPublisher.CurrentRepository.Items[0].SynchItemData.Identifier);
            Assert.AreEqual(@"Data\Test\F1", pushPublisher.CurrentRepository.Items[0].Items[0].SynchItemData.Identifier);
            Assert.AreEqual(@"Data\Test\F1\F11", pushPublisher.CurrentRepository.Items[0].Items[0].Items[0].SynchItemData.Identifier);
            Assert.AreEqual(@"Data\Test\F1\F11\F12", pushPublisher.CurrentRepository.Items[0].Items[0].Items[0].Items[0].SynchItemData.Identifier);
            Assert.AreEqual(@"Data\Test\F1\F11\F12\F12.xml", pushPublisher.CurrentRepository.Items[0].Items[0].Items[0].Items[0].Items[0].SynchItemData.Identifier);
        }
        [Test]
        public void UpdateCurrentRepository_Insert_Duplicated_Ok()
        {
            PushPublisher pushPublisher = new PushPublisher();
            pushPublisher.Initialize(new BrokerClient(), new MySynchPublisherConfigurationSection { LocalRootFolder = @"Data" }, "my host url");
            pushPublisher.UpdateCurrentRepository(@"Data\Test\F1\F11\F12\F12.xml", OperationType.Insert);
            Assert.False(pushPublisher.UpdateCurrentRepository(@"Data\Test\F1\F11\F12\F12.xml", OperationType.Insert));
            Assert.IsNotNull(pushPublisher.CurrentRepository);
            Assert.AreEqual(1, pushPublisher.CurrentRepository.Items.Count);
            Assert.AreEqual(@"Data", pushPublisher.CurrentRepository.SynchItemData.Identifier);
            Assert.AreEqual(@"Data\Test", pushPublisher.CurrentRepository.Items[0].SynchItemData.Identifier);
            Assert.AreEqual(@"Data\Test\F1", pushPublisher.CurrentRepository.Items[0].Items[0].SynchItemData.Identifier);
            Assert.AreEqual(@"Data\Test\F1\F11", pushPublisher.CurrentRepository.Items[0].Items[0].Items[0].SynchItemData.Identifier);
            Assert.AreEqual(@"Data\Test\F1\F11\F12", pushPublisher.CurrentRepository.Items[0].Items[0].Items[0].Items[0].SynchItemData.Identifier);
            Assert.AreEqual(@"Data\Test\F1\F11\F12\F12.xml", pushPublisher.CurrentRepository.Items[0].Items[0].Items[0].Items[0].Items[0].SynchItemData.Identifier);
        }

        [Test]
        public void UpdateCurrentRepository_Update_Ok()
        {
            PushPublisher pushPublisher = new PushPublisher();
            pushPublisher.Initialize(new BrokerClient(), new MySynchPublisherConfigurationSection{LocalRootFolder = @"c:\code\Sciendo\MySynch\MySynch.Tests\Data" }, "my host url");
            pushPublisher.UpdateCurrentRepository(@"c:\code\Sciendo\MySynch\MySynch.Tests\Data\items.xml",OperationType.Insert);
            pushPublisher.CurrentRepository.Items[0].SynchItemData.Size = 2;
            Assert.True(pushPublisher.UpdateCurrentRepository(@"c:\code\Sciendo\MySynch\MySynch.Tests\Data\items.xml", OperationType.Update));
            Assert.IsNotNull(pushPublisher.CurrentRepository);
            Assert.AreEqual(1, pushPublisher.CurrentRepository.Items.Count);
            Assert.AreEqual(@"c:\code\Sciendo\MySynch\MySynch.Tests\Data\items.xml", pushPublisher.CurrentRepository.Items[0].SynchItemData.Identifier);
            Assert.Greater(pushPublisher.CurrentRepository.Items[0].SynchItemData.Size,2);
        }

        [Test]
        public void UpdateCurrentRepository_Update_NoChange_AfterInsertOk()
        {
            PushPublisher pushPublisher = new PushPublisher();
            pushPublisher.Initialize(new BrokerClient(), new MySynchPublisherConfigurationSection { LocalRootFolder = @"c:\code\Sciendo\MySynch\MySynch.Tests\Data" }, "my host url");
            pushPublisher.UpdateCurrentRepository(@"c:\code\Sciendo\MySynch\MySynch.Tests\Data\items.xml", OperationType.Insert);
            Assert.False(pushPublisher.UpdateCurrentRepository(@"c:\code\Sciendo\MySynch\MySynch.Tests\Data\items.xml", OperationType.Update));
            Assert.IsNotNull(pushPublisher.CurrentRepository);
            Assert.AreEqual(1, pushPublisher.CurrentRepository.Items.Count);
            Assert.AreEqual(@"c:\code\Sciendo\MySynch\MySynch.Tests\Data\items.xml", pushPublisher.CurrentRepository.Items[0].SynchItemData.Identifier);
            Assert.Greater(pushPublisher.CurrentRepository.Items[0].SynchItemData.Size, 2);
        }

        [Test]
        public void UpdateCurrentRepository_Delete_Ok()
        {
            PushPublisher pushPublisher = new PushPublisher();
            pushPublisher.Initialize(new BrokerClient(), new MySynchPublisherConfigurationSection { LocalRootFolder = @"c:\code\Sciendo\MySynch\MySynch.Tests\Data" }, "my host url");
            Assert.True(pushPublisher.UpdateCurrentRepository(@"c:\code\Sciendo\MySynch\MySynch.Tests\Data\items.xml", OperationType.Insert));
            Assert.True(pushPublisher.UpdateCurrentRepository(@"c:\code\Sciendo\MySynch\MySynch.Tests\Data\items.xml", OperationType.Delete));
            Assert.IsNotNull(pushPublisher.CurrentRepository);
            Assert.AreEqual(0, pushPublisher.CurrentRepository.Items.Count);
        }

        [Test]
        public void ProcessOperation_NothingSent()
        {
            PushPublisher pushPublisher = new PushPublisher();
            pushPublisher.ProcessOperation(null,OperationType.None);
        }
    }
}
