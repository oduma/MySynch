﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MySynch.Contracts.Messages;
using MySynch.Core;
using MySynch.Core.Publisher;
using NUnit.Framework;

namespace MySynch.Tests
{
    [TestFixture]
    public class ChangePublisherTests
    {
        private PublishPackageRequestResponse _expectedPackageRequestResponse;

        private ChangePublisher _changePublisherForThreads;

        [SetUp]
        public void SetUp()
        {
            if (File.Exists("backup.xml"))
            {
                File.Copy("backup.xml", "backup1.xml", true);
                File.Delete("backup.xml");
            }

            _expectedPackageRequestResponse = new PublishPackageRequestResponse { Source = Environment.MachineName, SourceRootName = "source root name 1" };
            _changePublisherForThreads= new ChangePublisher();
            var mockItemDiscoverer = MockTestHelper.MockItemDiscoverer("source root name 1");
            _changePublisherForThreads.Initialize("source root name 1", mockItemDiscoverer);
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists("backup1.xml"))
            {
                File.Copy("backup1.xml", "backup.xml", true);
                File.Delete("backup1.xml");
            }

        }
        [Test]
        public void QueueOneOperationAtATime_Ok()
        {
            ChangePublisher changePublisher = new ChangePublisher();
            var mockItemDiscoverer = MockTestHelper.MockItemDiscoverer("source root name 1");
            changePublisher.Initialize("source root name 1", mockItemDiscoverer);

            changePublisher.QueueInsert("item one");

            _expectedPackageRequestResponse.ChangePushItems = new List<ChangePushItem>
                                                   {
                                                       new ChangePushItem
                                                           {
                                                               AbsolutePath = "item one",
                                                               OperationType = OperationType.Insert
                                                           }
                                                   };
            var actualPackage = changePublisher.PublishPackage();
            CompareTwoPackages(_expectedPackageRequestResponse, actualPackage);
            changePublisher.RemovePackage(actualPackage);
            Assert.IsNull(changePublisher.PublishPackage());

            changePublisher.QueueUpdate("item one");

            _expectedPackageRequestResponse.ChangePushItems = new List<ChangePushItem>
                                                   {
                                                       new ChangePushItem
                                                           {
                                                               AbsolutePath = "item one",
                                                               OperationType = OperationType.Update
                                                           }
                                                   };
            actualPackage = changePublisher.PublishPackage();
            CompareTwoPackages(_expectedPackageRequestResponse, actualPackage);
            changePublisher.RemovePackage(actualPackage); 
            Assert.IsNull(changePublisher.PublishPackage());

            changePublisher.QueueDelete("item one");
            _expectedPackageRequestResponse.ChangePushItems = new List<ChangePushItem>
                                                   {
                                                       new ChangePushItem
                                                           {
                                                               AbsolutePath = "item one",
                                                               OperationType = OperationType.Delete
                                                           }
                                                   };
            actualPackage = changePublisher.PublishPackage();
            CompareTwoPackages(_expectedPackageRequestResponse, actualPackage);
            changePublisher.RemovePackage(actualPackage); 
            Assert.IsNull(changePublisher.PublishPackage());

        }

        private void CompareTwoPackages(PublishPackageRequestResponse expectedPackageRequestResponse, PublishPackageRequestResponse actualPackageRequestResponse)
        {
            Assert.AreEqual(expectedPackageRequestResponse.Source,actualPackageRequestResponse.Source);
            Assert.AreEqual(expectedPackageRequestResponse.SourceRootName, actualPackageRequestResponse.SourceRootName);
            Assert.AreEqual(expectedPackageRequestResponse.ChangePushItems.Count,actualPackageRequestResponse.ChangePushItems.Count);
            foreach (var expectedItem in expectedPackageRequestResponse.ChangePushItems)
            {
                Assert.IsNotNull(actualPackageRequestResponse.ChangePushItems.FirstOrDefault(i=>i.AbsolutePath==expectedItem.AbsolutePath && i.OperationType==expectedItem.OperationType));
            }
        }

        [Test]
        public void QueueOperationsSomeDuplicates_NonThreaded_Ok()
        {
            ChangePublisher changePublisher = new ChangePublisher();
            var mockItemDiscoverer = MockTestHelper.MockItemDiscoverer("source root name 1");
            changePublisher.Initialize("source root name 1",mockItemDiscoverer);

            changePublisher.QueueInsert("item one");

            changePublisher.QueueUpdate("item one");

            _expectedPackageRequestResponse.ChangePushItems = new List<ChangePushItem>
                                                   {
                                                       new ChangePushItem
                                                           {
                                                               AbsolutePath = "item one",
                                                               OperationType = OperationType.Update
                                                           }
                                                   };
            CompareTwoPackages(_expectedPackageRequestResponse, changePublisher.PublishPackage());
        }

        [Test]
        [Ignore("Have to fix it")]
        public void QueueOperationsSomeDuplicates_Threaded_Ok()
        {
            

            Task taskInsert = new Task(InitiateAnInsert);
            Task taskUpdate= new Task(InitiateAnUpdate);

            _expectedPackageRequestResponse.ChangePushItems = new List<ChangePushItem>
                                                   {
                                                       new ChangePushItem
                                                           {
                                                               AbsolutePath = "item one",
                                                               OperationType = OperationType.Update
                                                           }
                                                   };
            Task<PublishPackageRequestResponse> publisher= new Task<PublishPackageRequestResponse>(_changePublisherForThreads.PublishPackage);
            publisher.Start();
            Assert.AreEqual(0, publisher.Result.ChangePushItems.Count);
            publisher = new Task<PublishPackageRequestResponse>(_changePublisherForThreads.PublishPackage);

            taskInsert.Start();
            taskUpdate.Start();
            publisher.Start();
            var itemsPushed = publisher.Result.ChangePushItems;
            Assert.AreEqual(1, itemsPushed.Count);
            var secondRound = _changePublisherForThreads.PublishPackage();
            if (secondRound.ChangePushItems.Count > 0)
                Assert.AreNotEqual(itemsPushed[0].OperationType, secondRound.ChangePushItems[0].OperationType);
        }

        private void InitiateAnUpdate()
        {
            _changePublisherForThreads.QueueUpdate("item one");
        }

        private void InitiateAnInsert()
        {
            _changePublisherForThreads.QueueInsert("item one");
        }

        [Test]
        public void QueueInsert_Nothing_Sent()
        {
            ChangePublisher changePublisher = new ChangePublisher();
            var mockItemDiscoverer = MockTestHelper.MockItemDiscoverer("source root name 1");
            changePublisher.Initialize("source root name 1",mockItemDiscoverer);

            changePublisher.QueueInsert("");

            Assert.IsNull(changePublisher.PublishPackage());
        }
        [Test]
        public void QueueUpdate_Nothing_Sent()
        {
            ChangePublisher changePublisher = new ChangePublisher( );
            var mockItemDiscoverer = MockTestHelper.MockItemDiscoverer("source root name 1");
            changePublisher.Initialize("source root name 1", mockItemDiscoverer);

            changePublisher.QueueUpdate(null);

            _expectedPackageRequestResponse.ChangePushItems = new List<ChangePushItem>();

            Assert.IsNull(changePublisher.PublishPackage());
        }
        [Test]
        public void QueueDelete_Nothing_Sent()
        {
            ChangePublisher changePublisher = new ChangePublisher( );
            var mockItemDiscoverer = MockTestHelper.MockItemDiscoverer("source root name 1");

            changePublisher.Initialize("source root name 1",mockItemDiscoverer);

            changePublisher.QueueDelete(string.Empty);

            Assert.IsNull(changePublisher.PublishPackage());
        }

        [Test]
        [ExpectedException(typeof(PublisherSetupException))]
        public void PublishPackage_Not_Enough_Info()
        {
            ChangePublisher changePublisher = new ChangePublisher();

            changePublisher.QueueDelete(string.Empty);

            changePublisher.PublishPackage();
        }

        [Test]
        public void RemovePackage_Ok()
        {
            ChangePublisher changePublisher = new ChangePublisher();
            var mockItemDiscoverer = MockTestHelper.MockItemDiscoverer("source root name 1");

            changePublisher.Initialize("source root name 1",mockItemDiscoverer);

            changePublisher.QueueInsert("item one");

            changePublisher.QueueUpdate("item one");

            _expectedPackageRequestResponse.ChangePushItems = new List<ChangePushItem>
                                                   {
                                                       new ChangePushItem
                                                           {
                                                               AbsolutePath = "item one",
                                                               OperationType = OperationType.Update
                                                           }
                                                   };
            var publishedPackage = changePublisher.PublishPackage();

            CompareTwoPackages(_expectedPackageRequestResponse,publishedPackage);

            Assert.AreEqual(1, changePublisher.PublishedPackageNotDistributed.Count);
            Assert.AreEqual(1,changePublisher.PublishedPackageNotDistributed[0].ChangePushItems.Count);
            Assert.AreEqual(publishedPackage.PackageId,changePublisher.PublishedPackageNotDistributed[0].PackageId);
            changePublisher.RemovePackage(publishedPackage);
            Assert.AreEqual(0,changePublisher.PublishedPackageNotDistributed.Count);
        }

        [Test]
        public void RemovePackage_AfterPackage_Changed()
        {
            ChangePublisher changePublisher = new ChangePublisher();
            var mockItemDiscoverer = MockTestHelper.MockItemDiscoverer("source root name 1");

            changePublisher.Initialize("source root name 1",mockItemDiscoverer);

            changePublisher.QueueInsert("item one");

            changePublisher.QueueUpdate("item one");

            _expectedPackageRequestResponse.ChangePushItems = new List<ChangePushItem>
                                                   {
                                                       new ChangePushItem
                                                           {
                                                               AbsolutePath = "item one",
                                                               OperationType = OperationType.Update
                                                           }
                                                   };
            var firstPublishedPackage = changePublisher.PublishPackage();

            CompareTwoPackages(_expectedPackageRequestResponse, firstPublishedPackage);

            Assert.AreEqual(1, changePublisher.PublishedPackageNotDistributed.Count);
            Assert.AreEqual(1, changePublisher.PublishedPackageNotDistributed[0].ChangePushItems.Count);

            changePublisher.QueueDelete("item one");
            changePublisher.QueueInsert("item two");

            _expectedPackageRequestResponse.ChangePushItems = new List<ChangePushItem>
                                                   {
                                                       new ChangePushItem
                                                           {
                                                               AbsolutePath = "item one",
                                                               OperationType = OperationType.Delete
                                                           },
                                                       new ChangePushItem
                                                           {
                                                               AbsolutePath = "item two",
                                                               OperationType = OperationType.Insert
                                                           }
                                                   };

            changePublisher.RemovePackage(firstPublishedPackage);
            var scndPublishedPackage = changePublisher.PublishPackage();

            CompareTwoPackages(_expectedPackageRequestResponse, scndPublishedPackage);
            Assert.AreEqual(1, changePublisher.PublishedPackageNotDistributed.Count);
            Assert.AreEqual(2, changePublisher.PublishedPackageNotDistributed[0].ChangePushItems.Count);
            Assert.AreEqual(scndPublishedPackage.PackageId, changePublisher.PublishedPackageNotDistributed[0].PackageId);
        }
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemovePackage_NoPackageSent()
        {
            ChangePublisher changePublisher = new ChangePublisher();
            var mockItemDiscoverer = MockTestHelper.MockItemDiscoverer("source root name 1");

            changePublisher.Initialize("source root name 1",mockItemDiscoverer);

            changePublisher.RemovePackage(null);
        }
    }
}
