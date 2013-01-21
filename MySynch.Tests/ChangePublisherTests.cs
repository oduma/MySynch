using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySynch.Core;
using MySynch.Core.DataTypes;
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
            _changePublisherForThreads.Initialize("source root name 1");
        }

        [Test]
        public void QueueOneOperationAtATime_Ok()
        {
            ChangePublisher changePublisher = new ChangePublisher();
            changePublisher.Initialize("source root name 1");

            changePublisher.QueueInsert("item one");

            _expectedPackage.ChangePushItems = new List<ChangePushItem>
                                                   {
                                                       new ChangePushItem
                                                           {
                                                               AbsolutePath = "item one",
                                                               OperationType = OperationType.Insert
                                                           }
                                                   };
            CompareTwoPackages(_expectedPackage, changePublisher.PublishPackage());
            Assert.AreEqual(0,changePublisher.PublishPackage().ChangePushItems.Count);

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
            Assert.AreEqual(0, changePublisher.PublishPackage().ChangePushItems.Count);

            changePublisher.QueueDelete("item one");
            _expectedPackage.ChangePushItems = new List<ChangePushItem>
                                                   {
                                                       new ChangePushItem
                                                           {
                                                               AbsolutePath = "item one",
                                                               OperationType = OperationType.Delete
                                                           }
                                                   };
            CompareTwoPackages(_expectedPackage, changePublisher.PublishPackage());
            Assert.AreEqual(0, changePublisher.PublishPackage().ChangePushItems.Count);

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
            changePublisher.Initialize("source root name 1");

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
            Assert.AreEqual(0, changePublisher.PublishPackage().ChangePushItems.Count);
        }

        [Test]
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
            changePublisher.Initialize("source root name 1");

            changePublisher.QueueInsert("");

            _expectedPackage.ChangePushItems = new List<ChangePushItem>();

            CompareTwoPackages(_expectedPackage, changePublisher.PublishPackage());
            Assert.AreEqual(0, changePublisher.PublishPackage().ChangePushItems.Count);
        }
        [Test]
        public void QueueUpdate_Nothing_Sent()
        {
            ChangePublisher changePublisher = new ChangePublisher( );
            changePublisher.Initialize("source root name 1");

            changePublisher.QueueUpdate(null);

            _expectedPackage.ChangePushItems = new List<ChangePushItem>();

            CompareTwoPackages(_expectedPackage, changePublisher.PublishPackage());
            Assert.AreEqual(0, changePublisher.PublishPackage().ChangePushItems.Count);
        }
        [Test]
        public void QueueDelete_Nothing_Sent()
        {
            ChangePublisher changePublisher = new ChangePublisher( );

            changePublisher.Initialize("source root name 1");

            changePublisher.QueueDelete(string.Empty);

            _expectedPackage.ChangePushItems = new List<ChangePushItem>();

            CompareTwoPackages(_expectedPackage, changePublisher.PublishPackage());
            Assert.AreEqual(0, changePublisher.PublishPackage().ChangePushItems.Count);
        }

        [Test]
        [ExpectedException(typeof(PublisherSetupException))]
        public void PublishPackage_Not_Enough_Info()
        {
            ChangePublisher changePublisher = new ChangePublisher();

            changePublisher.QueueDelete(string.Empty);

            changePublisher.PublishPackage();
        }
    }
}
