using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySynch.Contracts.Messages;
using MySynch.Core;
using NUnit.Framework;

namespace MySynch.Tests
{
    [TestFixture]
    public class ChangePublisherTests
    {
        private ChangePushPackage _expectedPackage;

        private ChangePublisher _changePublisherForThreads;

        [SetUp]
        public void SetUp()
        {
            _expectedPackage = new ChangePushPackage { Source = Environment.MachineName, SourceRootName = "source root name 1" };
            _changePublisherForThreads= new ChangePublisher();
            var mockItemDiscoverer = MockTestHelper.MockItemDiscoverer("source root name 1");
            _changePublisherForThreads.Initialize("source root name 1", mockItemDiscoverer);
        }
        [Test]
        public void QueueOneOperationAtATime_Ok()
        {
            ChangePublisher changePublisher = new ChangePublisher();
            var mockItemDiscoverer = MockTestHelper.MockItemDiscoverer("source root name 1");
            changePublisher.Initialize("source root name 1", mockItemDiscoverer);

            changePublisher.QueueInsert("item one");

            _expectedPackage.ChangePushItems = new List<ChangePushItem>
                                                   {
                                                       new ChangePushItem
                                                           {
                                                               AbsolutePath = "item one",
                                                               OperationType = OperationType.Insert
                                                           }
                                                   };
            var actualPackage = changePublisher.PublishPackage();
            CompareTwoPackages(_expectedPackage, actualPackage);
            changePublisher.RemovePackage(actualPackage);
            Assert.IsNull(changePublisher.PublishPackage());

            changePublisher.QueueUpdate("item one");

            _expectedPackage.ChangePushItems = new List<ChangePushItem>
                                                   {
                                                       new ChangePushItem
                                                           {
                                                               AbsolutePath = "item one",
                                                               OperationType = OperationType.Update
                                                           }
                                                   };
            actualPackage = changePublisher.PublishPackage();
            CompareTwoPackages(_expectedPackage, actualPackage);
            changePublisher.RemovePackage(actualPackage); 
            Assert.IsNull(changePublisher.PublishPackage());

            changePublisher.QueueDelete("item one");
            _expectedPackage.ChangePushItems = new List<ChangePushItem>
                                                   {
                                                       new ChangePushItem
                                                           {
                                                               AbsolutePath = "item one",
                                                               OperationType = OperationType.Delete
                                                           }
                                                   };
            actualPackage = changePublisher.PublishPackage();
            CompareTwoPackages(_expectedPackage, actualPackage);
            changePublisher.RemovePackage(actualPackage); 
            Assert.IsNull(changePublisher.PublishPackage());

        }

        private void CompareTwoPackages(ChangePushPackage expectedPackage, ChangePushPackage actualPackage)
        {
            Assert.AreEqual(expectedPackage.Source,actualPackage.Source);
            Assert.AreEqual(expectedPackage.SourceRootName, actualPackage.SourceRootName);
            Assert.AreEqual(expectedPackage.ChangePushItems.Count,actualPackage.ChangePushItems.Count);
            foreach (var expectedItem in expectedPackage.ChangePushItems)
            {
                Assert.IsNotNull(actualPackage.ChangePushItems.FirstOrDefault(i=>i.AbsolutePath==expectedItem.AbsolutePath && i.OperationType==expectedItem.OperationType));
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

            _expectedPackage.ChangePushItems = new List<ChangePushItem>
                                                   {
                                                       new ChangePushItem
                                                           {
                                                               AbsolutePath = "item one",
                                                               OperationType = OperationType.Update
                                                           }
                                                   };
            CompareTwoPackages(_expectedPackage, changePublisher.PublishPackage());
        }

        [Test]
        [Ignore("Have to fix it")]
        public void QueueOperationsSomeDuplicates_Threaded_Ok()
        {
            

            Task taskInsert = new Task(InitiateAnInsert);
            Task taskUpdate= new Task(InitiateAnUpdate);

            _expectedPackage.ChangePushItems = new List<ChangePushItem>
                                                   {
                                                       new ChangePushItem
                                                           {
                                                               AbsolutePath = "item one",
                                                               OperationType = OperationType.Update
                                                           }
                                                   };
            Task<ChangePushPackage> publisher= new Task<ChangePushPackage>(_changePublisherForThreads.PublishPackage);
            publisher.Start();
            Assert.AreEqual(0, publisher.Result.ChangePushItems.Count);
            publisher = new Task<ChangePushPackage>(_changePublisherForThreads.PublishPackage);

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

            _expectedPackage.ChangePushItems = new List<ChangePushItem>();

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

            _expectedPackage.ChangePushItems = new List<ChangePushItem>
                                                   {
                                                       new ChangePushItem
                                                           {
                                                               AbsolutePath = "item one",
                                                               OperationType = OperationType.Update
                                                           }
                                                   };
            var publishedPackage = changePublisher.PublishPackage();

            CompareTwoPackages(_expectedPackage,publishedPackage);

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

            _expectedPackage.ChangePushItems = new List<ChangePushItem>
                                                   {
                                                       new ChangePushItem
                                                           {
                                                               AbsolutePath = "item one",
                                                               OperationType = OperationType.Update
                                                           }
                                                   };
            var firstPublishedPackage = changePublisher.PublishPackage();

            CompareTwoPackages(_expectedPackage, firstPublishedPackage);

            Assert.AreEqual(1, changePublisher.PublishedPackageNotDistributed.Count);
            Assert.AreEqual(1, changePublisher.PublishedPackageNotDistributed[0].ChangePushItems.Count);

            changePublisher.QueueDelete("item one");
            changePublisher.QueueInsert("item two");

            _expectedPackage.ChangePushItems = new List<ChangePushItem>
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

            CompareTwoPackages(_expectedPackage, scndPublishedPackage);
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
