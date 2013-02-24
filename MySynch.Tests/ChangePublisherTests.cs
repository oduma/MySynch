using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using MySynch.Contracts.Messages;
using MySynch.Core;
using MySynch.Core.DataTypes;
using MySynch.Core.Interfaces;
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
            var mockItemDiscoverer = MockItemDiscoverer("source root name 1");
            _changePublisherForThreads.Initialize("source root name 1", mockItemDiscoverer);
        }

        private static IItemDiscoverer MockItemDiscoverer(string folderPath)
        {
            var mockItemDiscoverer = new Mock<IItemDiscoverer>();
            if (folderPath == "generate backup")
                mockItemDiscoverer.Setup(m => m.DiscoverFromFolder(folderPath)).Returns(
                new SynchItem{SynchItemData = new SynchItemData{Name="root",Identifier="root"},Items=new List<SynchItem>{
                    new SynchItem{SynchItemData=new SynchItemData{Name="100",Identifier=@"root\100"},Items=new List<SynchItem>{
                        new SynchItem{SynchItemData=new SynchItemData{Name="110",Identifier=@"root\100\110"},Items=new List<SynchItem>{
                            new SynchItem{SynchItemData=new SynchItemData{Name="111",Identifier=@"root\100\110\111"}},
                            new SynchItem{SynchItemData=new SynchItemData{Name="112",Identifier=@"root\100\110\112"}}}},
                        new SynchItem{SynchItemData=new SynchItemData{Name="120",Identifier=@"root\100\120"},Items=new List<SynchItem>{
                            new SynchItem{SynchItemData=new SynchItemData{Name="121",Identifier=@"root\100\120\121"}},
                            new SynchItem{SynchItemData=new SynchItemData{Name="122",Identifier=@"root\100\120\122"}},
                            new SynchItem{SynchItemData=new SynchItemData{Name="123",Identifier=@"root\100\120\123"}}}}}},
                    new SynchItem{SynchItemData=new SynchItemData{Name="200",Identifier=@"root\200"}},
                    new SynchItem{SynchItemData=new SynchItemData{Name="300",Identifier=@"root\300"},Items=new List<SynchItem>{
                        new SynchItem{SynchItemData=new SynchItemData{Name="310",Identifier=@"root\300\310",Size=1}},
                        new SynchItem{SynchItemData=new SynchItemData{Name="320",Identifier=@"root\300\320"}},
                        new SynchItem{SynchItemData=new SynchItemData{Name="330",Identifier=@"root\300\330"}, Items=new List<SynchItem>{
                            new SynchItem{SynchItemData=new SynchItemData{Name="331",Identifier=@"root\300\330\331"},Items=new List<SynchItem>{
                                new SynchItem{SynchItemData=new SynchItemData{Name="331",Identifier=@"root\300\330\331\331"}}}},
                            new SynchItem{SynchItemData=new SynchItemData{Name="332", Identifier=@"root\300\330\332"}, Items= new List<SynchItem>{
                                new SynchItem{SynchItemData=new SynchItemData{Name="332", Identifier=@"root\300\330\332\332"}},
                                new SynchItem{SynchItemData=new SynchItemData{Name="333", Identifier=@"root\300\330\332\333"}}}}}}}}}});
            else
                mockItemDiscoverer.Setup(m => m.DiscoverFromFolder(folderPath)).Returns(new SynchItem());
            return mockItemDiscoverer.Object;
        }

        [Test]
        public void QueueOneOperationAtATime_Ok()
        {
            ChangePublisher changePublisher = new ChangePublisher();
            var mockItemDiscoverer = MockItemDiscoverer("source root name 1");
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
            var mockItemDiscoverer = MockItemDiscoverer("source root name 1");
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
            var mockItemDiscoverer = MockItemDiscoverer("source root name 1");
            changePublisher.Initialize("source root name 1",mockItemDiscoverer);

            changePublisher.QueueInsert("");

            Assert.IsNull(changePublisher.PublishPackage());
        }
        [Test]
        public void QueueUpdate_Nothing_Sent()
        {
            ChangePublisher changePublisher = new ChangePublisher( );
            var mockItemDiscoverer = MockItemDiscoverer("source root name 1");
            changePublisher.Initialize("source root name 1", mockItemDiscoverer);

            changePublisher.QueueUpdate(null);

            _expectedPackage.ChangePushItems = new List<ChangePushItem>();

            Assert.IsNull(changePublisher.PublishPackage());
        }
        [Test]
        public void QueueDelete_Nothing_Sent()
        {
            ChangePublisher changePublisher = new ChangePublisher( );
            var mockItemDiscoverer = MockItemDiscoverer("source root name 1");

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
            var mockItemDiscoverer = MockItemDiscoverer("source root name 1");

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
            var mockItemDiscoverer = MockItemDiscoverer("source root name 1");

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
            var mockItemDiscoverer = MockItemDiscoverer("source root name 1");

            changePublisher.Initialize("source root name 1",mockItemDiscoverer);

            changePublisher.RemovePackage(null);
        }

        [Test]
        public void GetOfflineChanges_ItemsToBeInserted()
        {
            ChangePublisher changePublisher= new ChangePublisher();
            SynchItem newTree = new SynchItem
            {
                SynchItemData = new SynchItemData { Name = "root", Identifier = "root" },
                Items = new List<SynchItem>{
                    new SynchItem{SynchItemData=new SynchItemData{Name="100",Identifier=@"root\100"},Items=new List<SynchItem>{
                        new SynchItem{SynchItemData=new SynchItemData{Name="110",Identifier=@"root\100\110"},Items=new List<SynchItem>{
                            new SynchItem{SynchItemData=new SynchItemData{Name="111",Identifier=@"root\100\110\113"}},
                            new SynchItem{SynchItemData=new SynchItemData{Name="112",Identifier=@"root\100\110\114"}}}},
                        new SynchItem{SynchItemData=new SynchItemData{Name="120",Identifier=@"root\100\120"},Items=new List<SynchItem>{
                            new SynchItem{SynchItemData=new SynchItemData{Name="121",Identifier=@"root\100\120\124"}},
                            new SynchItem{SynchItemData=new SynchItemData{Name="122",Identifier=@"root\100\120\125"}},
                            new SynchItem{SynchItemData=new SynchItemData{Name="123",Identifier=@"root\100\120\126"}}}}}},
                    new SynchItem{SynchItemData=new SynchItemData{Name="200",Identifier=@"root\200"}},
                    new SynchItem{SynchItemData=new SynchItemData{Name="300",Identifier=@"root\300"},Items=new List<SynchItem>{
                        new SynchItem{SynchItemData=new SynchItemData{Name="310",Identifier=@"root\300\310",Size=1}},
                        new SynchItem{SynchItemData=new SynchItemData{Name="320",Identifier=@"root\300\320"}},
                        new SynchItem{SynchItemData=new SynchItemData{Name="330",Identifier=@"root\300\330"}, Items=new List<SynchItem>{
                            new SynchItem{SynchItemData=new SynchItemData{Name="331",Identifier=@"root\300\330\333"},Items=new List<SynchItem>{
                                new SynchItem{SynchItemData=new SynchItemData{Name="331",Identifier=@"root\300\330\333\331"}}}},
                            new SynchItem{SynchItemData=new SynchItemData{Name="332", Identifier=@"root\300\330\334"}, Items= new List<SynchItem>{
                                new SynchItem{SynchItemData=new SynchItemData{Name="332", Identifier=@"root\300\330\334\332"}},
                                new SynchItem{SynchItemData=new SynchItemData{Name="333", Identifier=@"root\300\330\334\333"}}}}}}}}}
            };
            var listOfChanges = changePublisher.GetOfflineChanges(newTree);
            Assert.IsNotNull(listOfChanges);
            Assert.AreEqual(listOfChanges.Count,listOfChanges.Count(c=>c.Value==OperationType.Insert));
            Assert.AreEqual(8,listOfChanges.Count);

        }

        [Test]
        public void GetOfflineChanges_ItemsToBeDeleted()
        {
            Assert.Fail();
        }

        [Test]
        public void GetOfflineChanges_ItemsUpdated()
        {
            Assert.Fail();
        }

        [Test]
        public void GetOfflineChanges_MixtureOfItems()
        {
            Assert.Fail();
        }

        [Test]
        public void GetOfflineChanges_NoBackupFile()
        {
            Assert.Fail();
        }

        [Test]
        public void GetOfflineChanges_ItemsNoItemsInTheCurrentRepository()
        {
            Assert.Fail();
        }

        [Test]
        public void GetOfflineChanges_NoCurrentRepositorySent()
        {
            Assert.Fail();
        }

        [Test]
        public void GetOfflineChanges_WrongBackup()
        {
            Assert.Fail();
        }

        [Test]
        public void SaveBackup_OK()
        {
            ChangePublisher changePublisher = new ChangePublisher();
            var mockItemDiscoverer = MockItemDiscoverer("generate backup");
            changePublisher.Initialize("generate backup",mockItemDiscoverer);
            changePublisher.SaveSettingsEndExit();
        }
    }
}
