using System;
using System.Collections.Generic;
using System.Linq;
using MySynch.Common.Serialization;
using MySynch.Contracts.Messages;
using MySynch.Core.DataTypes;
using MySynch.Core.Publisher;
using NUnit.Framework;

namespace MySynch.Tests
{
    [TestFixture]
    public class OfflineChangesDetectorTests
    {

        [Test]
        public void GetOfflineChanges_ItemsToBeInserted()
        {
            SynchItem newTree = new SynchItem
            {
                SynchItemData = new SynchItemData { Name = "root", Identifier = "root" },
                Items = new List<SynchItem>{
                    new SynchItem{SynchItemData=new SynchItemData{Name="100",Identifier=@"root\100"},Items=new List<SynchItem>{
                        new SynchItem{SynchItemData=new SynchItemData{Name="110",Identifier=@"root\100\110"},Items=new List<SynchItem>{
                            new SynchItem{SynchItemData=new SynchItemData{Name="111",Identifier=@"root\100\110\111"}},
                            new SynchItem{SynchItemData=new SynchItemData{Name="111",Identifier=@"root\100\110\112"}},
                            new SynchItem{SynchItemData=new SynchItemData{Name="111",Identifier=@"root\100\110\113"}},
                            new SynchItem{SynchItemData=new SynchItemData{Name="112",Identifier=@"root\100\110\114"}}}},
                        new SynchItem{SynchItemData=new SynchItemData{Name="120",Identifier=@"root\100\120"},Items=new List<SynchItem>{
                            new SynchItem{SynchItemData=new SynchItemData{Name="121",Identifier=@"root\100\120\121"}},
                            new SynchItem{SynchItemData=new SynchItemData{Name="121",Identifier=@"root\100\120\122"}},
                            new SynchItem{SynchItemData=new SynchItemData{Name="121",Identifier=@"root\100\120\123"}},
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
                            new SynchItem{SynchItemData=new SynchItemData{Name="331",Identifier=@"root\300\330\331"},Items=new List<SynchItem>{
                                new SynchItem{SynchItemData=new SynchItemData{Name="331",Identifier=@"root\300\330\331\331"}}}},
                            new SynchItem{SynchItemData=new SynchItemData{Name="332", Identifier=@"root\300\330\334"}, Items= new List<SynchItem>{
                                new SynchItem{SynchItemData=new SynchItemData{Name="332", Identifier=@"root\300\330\334\332"}},
                                new SynchItem{SynchItemData=new SynchItemData{Name="333", Identifier=@"root\300\330\334\333"}}}},
                        new SynchItem{SynchItemData=new SynchItemData{Name="332", Identifier=@"root\300\330\332"}, Items= new List<SynchItem>{
                                new SynchItem{SynchItemData=new SynchItemData{Name="332", Identifier=@"root\300\330\332\332"}},
                                new SynchItem{SynchItemData=new SynchItemData{Name="333", Identifier=@"root\300\330\332\333"}}}}}}}}}
            };
            var listOfChanges = OfflineChangesDetector.GetOfflineChanges(newTree,@"backup.xml");
            Assert.IsNotNull(listOfChanges);
            Assert.AreEqual(listOfChanges.Count, listOfChanges.Count(c => c.Value == OperationType.Insert));
            Assert.AreEqual(8, listOfChanges.Count);

        }

        [Test]
        public void GetOfflineChanges_ItemsToBeDeleted()
        {
            SynchItem newTree = 
                new SynchItem{SynchItemData = new SynchItemData{Name="root",Identifier="root"},Items=new List<SynchItem>{
                    new SynchItem{SynchItemData=new SynchItemData{Name="100",Identifier=@"root\100"},Items=new List<SynchItem>{
                        new SynchItem{SynchItemData=new SynchItemData{Name="110",Identifier=@"root\100\110"},Items=new List<SynchItem>{
                            new SynchItem{SynchItemData=new SynchItemData{Name="112",Identifier=@"root\100\110\112"}}}},
                        new SynchItem{SynchItemData=new SynchItemData{Name="120",Identifier=@"root\100\120"},Items=new List<SynchItem>{
                            new SynchItem{SynchItemData=new SynchItemData{Name="121",Identifier=@"root\100\120\121"}},
                            new SynchItem{SynchItemData=new SynchItemData{Name="122",Identifier=@"root\100\120\122"}},
                            new SynchItem{SynchItemData=new SynchItemData{Name="123",Identifier=@"root\100\120\123"}}}}}},
                    new SynchItem{SynchItemData=new SynchItemData{Name="200",Identifier=@"root\200"}},
                    new SynchItem{SynchItemData=new SynchItemData{Name="300",Identifier=@"root\300"},Items=new List<SynchItem>{
                        new SynchItem{SynchItemData=new SynchItemData{Name="330",Identifier=@"root\300\330"}, Items=new List<SynchItem>{
                            new SynchItem{SynchItemData=new SynchItemData{Name="331",Identifier=@"root\300\330\331"},Items=new List<SynchItem>{
                                new SynchItem{SynchItemData=new SynchItemData{Name="331",Identifier=@"root\300\330\331\331"}}}},
                            new SynchItem{SynchItemData=new SynchItemData{Name="332", Identifier=@"root\300\330\332"}, Items= new List<SynchItem>{
                                new SynchItem{SynchItemData=new SynchItemData{Name="332", Identifier=@"root\300\330\332\332"}}}}}}}}}};
            var listOfChanges = OfflineChangesDetector.GetOfflineChanges(newTree,@"backup.xml");
            Assert.IsNotNull(listOfChanges);
            Assert.AreEqual(listOfChanges.Count, listOfChanges.Count(c => c.Value == OperationType.Delete));
            Assert.AreEqual(4, listOfChanges.Count);
        }

        [Test]
        public void GetOfflineChanges_ItemsUpdated()
        {
            SynchItem newTree =
                new SynchItem{SynchItemData = new SynchItemData{Name="root",Identifier="root"},Items=new List<SynchItem>{
                    new SynchItem{SynchItemData=new SynchItemData{Name="100",Identifier=@"root\100"},Items=new List<SynchItem>{
                        new SynchItem{SynchItemData=new SynchItemData{Name="110",Identifier=@"root\100\110"},Items=new List<SynchItem>{
                            new SynchItem{SynchItemData=new SynchItemData{Name="111",Identifier=@"root\100\110\111",Size=2}},
                            new SynchItem{SynchItemData=new SynchItemData{Name="112",Identifier=@"root\100\110\112"}}}},
                        new SynchItem{SynchItemData=new SynchItemData{Name="120",Identifier=@"root\100\120"},Items=new List<SynchItem>{
                            new SynchItem{SynchItemData=new SynchItemData{Name="121",Identifier=@"root\100\120\121"}},
                            new SynchItem{SynchItemData=new SynchItemData{Name="122",Identifier=@"root\100\120\122"}},
                            new SynchItem{SynchItemData=new SynchItemData{Name="123",Identifier=@"root\100\120\123"}}}}}},
                    new SynchItem{SynchItemData=new SynchItemData{Name="200",Identifier=@"root\200"}},
                    new SynchItem{SynchItemData=new SynchItemData{Name="300",Identifier=@"root\300"},Items=new List<SynchItem>{
                        new SynchItem{SynchItemData=new SynchItemData{Name="310",Identifier=@"root\300\310",Size=0}},
                        new SynchItem{SynchItemData=new SynchItemData{Name="320",Identifier=@"root\300\320"}},
                        new SynchItem{SynchItemData=new SynchItemData{Name="330",Identifier=@"root\300\330"}, Items=new List<SynchItem>{
                            new SynchItem{SynchItemData=new SynchItemData{Name="331",Identifier=@"root\300\330\331"},Items=new List<SynchItem>{
                                new SynchItem{SynchItemData=new SynchItemData{Name="331",Identifier=@"root\300\330\331\331"}}}},
                            new SynchItem{SynchItemData=new SynchItemData{Name="332", Identifier=@"root\300\330\332"}, Items= new List<SynchItem>{
                                new SynchItem{SynchItemData=new SynchItemData{Name="332", Identifier=@"root\300\330\332\332"}},
                                new SynchItem{SynchItemData=new SynchItemData{Name="333", Identifier=@"root\300\330\332\333",Size=34}}}}}}}}}};

            var listOfChanges = OfflineChangesDetector.GetOfflineChanges(newTree,@"backup.xml");
            Assert.IsNotNull(listOfChanges);
            Assert.AreEqual(listOfChanges.Count, listOfChanges.Count(c => c.Value == OperationType.Update));
            Assert.AreEqual(3, listOfChanges.Count);
        }

        [Test]
        public void GetOfflineChanges_MixtureOfItems()
        {
            SynchItem newTree =
                new SynchItem
                {
                    SynchItemData = new SynchItemData { Name = "root", Identifier = "root" },
                    Items = new List<SynchItem>{
                    new SynchItem{SynchItemData=new SynchItemData{Name="100",Identifier=@"root\100"},Items=new List<SynchItem>{
                        new SynchItem{SynchItemData=new SynchItemData{Name="110",Identifier=@"root\100\110"},Items=new List<SynchItem>{
                            new SynchItem{SynchItemData=new SynchItemData{Name="111",Identifier=@"root\100\110\111",Size=2}},
                            new SynchItem{SynchItemData=new SynchItemData{Name="112",Identifier=@"root\100\110\112"}}}},
                        new SynchItem{SynchItemData=new SynchItemData{Name="120",Identifier=@"root\100\120"},Items=new List<SynchItem>{
                            new SynchItem{SynchItemData=new SynchItemData{Name="123",Identifier=@"root\100\120\123"}}}}}},
                    new SynchItem{SynchItemData=new SynchItemData{Name="200",Identifier=@"root\200"}},
                    new SynchItem{SynchItemData=new SynchItemData{Name="300",Identifier=@"root\300"},Items=new List<SynchItem>{
                        new SynchItem{SynchItemData=new SynchItemData{Name="310",Identifier=@"root\300\310",Size=0}},
                        new SynchItem{SynchItemData=new SynchItemData{Name="320",Identifier=@"root\300\320"}},
                        new SynchItem{SynchItemData=new SynchItemData{Name="330",Identifier=@"root\300\330"}, Items=new List<SynchItem>{
                            new SynchItem{SynchItemData=new SynchItemData{Name="331",Identifier=@"root\300\330\331"},Items=new List<SynchItem>{
                                new SynchItem{SynchItemData=new SynchItemData{Name="331",Identifier=@"root\300\330\331\331"}}}},
                            new SynchItem{SynchItemData=new SynchItemData{Name="332", Identifier=@"root\300\330\332"}, Items= new List<SynchItem>{
                                new SynchItem{SynchItemData=new SynchItemData{Name="332", Identifier=@"root\300\330\332\332"}},
                                new SynchItem{SynchItemData=new SynchItemData{Name="332", Identifier=@"root\300\330\332\334"}},
                                new SynchItem{SynchItemData=new SynchItemData{Name="332", Identifier=@"root\300\330\332\335"}},
                                new SynchItem{SynchItemData=new SynchItemData{Name="333", Identifier=@"root\300\330\332\333",Size=34}}}}}}}}}
                };

            var listOfChanges = OfflineChangesDetector.GetOfflineChanges(newTree,@"backup.xml");
            Assert.IsNotNull(listOfChanges);
            Assert.AreEqual(3, listOfChanges.Count(c => c.Value == OperationType.Update));
            Assert.AreEqual(2, listOfChanges.Count(c => c.Value == OperationType.Insert));
            Assert.AreEqual(2, listOfChanges.Count(c => c.Value == OperationType.Delete));
            Assert.AreEqual(7, listOfChanges.Count);
        }

        [Test]
        public void GetOfflineChanges_NoBackupFile()
        {
            SynchItem newTree =
                new SynchItem
                {
                    SynchItemData = new SynchItemData { Name = "root", Identifier = "root" },
                    Items = new List<SynchItem>{
                new SynchItem{SynchItemData=new SynchItemData{Name="100",Identifier=@"root\100"},Items=new List<SynchItem>{
                    new SynchItem{SynchItemData=new SynchItemData{Name="110",Identifier=@"root\100\110"},Items=new List<SynchItem>{
                        new SynchItem{SynchItemData=new SynchItemData{Name="111",Identifier=@"root\100\110\111",Size=2}},
                        new SynchItem{SynchItemData=new SynchItemData{Name="112",Identifier=@"root\100\110\112"}}}},
                    new SynchItem{SynchItemData=new SynchItemData{Name="120",Identifier=@"root\100\120"},Items=new List<SynchItem>{
                        new SynchItem{SynchItemData=new SynchItemData{Name="123",Identifier=@"root\100\120\123"}}}}}},
                new SynchItem{SynchItemData=new SynchItemData{Name="200",Identifier=@"root\200"}},
                new SynchItem{SynchItemData=new SynchItemData{Name="300",Identifier=@"root\300"},Items=new List<SynchItem>{
                    new SynchItem{SynchItemData=new SynchItemData{Name="310",Identifier=@"root\300\310",Size=0}},
                    new SynchItem{SynchItemData=new SynchItemData{Name="320",Identifier=@"root\300\320"}},
                    new SynchItem{SynchItemData=new SynchItemData{Name="330",Identifier=@"root\300\330"}, Items=new List<SynchItem>{
                        new SynchItem{SynchItemData=new SynchItemData{Name="331",Identifier=@"root\300\330\331"},Items=new List<SynchItem>{
                            new SynchItem{SynchItemData=new SynchItemData{Name="331",Identifier=@"root\300\330\331\331"}}}},
                        new SynchItem{SynchItemData=new SynchItemData{Name="332", Identifier=@"root\300\330\332"}, Items= new List<SynchItem>{
                            new SynchItem{SynchItemData=new SynchItemData{Name="332", Identifier=@"root\300\330\332\332"}},
                            new SynchItem{SynchItemData=new SynchItemData{Name="332", Identifier=@"root\300\330\332\334"}},
                            new SynchItem{SynchItemData=new SynchItemData{Name="332", Identifier=@"root\300\330\332\335"}},
                            new SynchItem{SynchItemData=new SynchItemData{Name="333", Identifier=@"root\300\330\332\333",Size=34}}}}}}}}}
                };

            var listOfChanges = OfflineChangesDetector.GetOfflineChanges(newTree,"nofile.xml");
            Assert.IsEmpty(listOfChanges);
        }

        [Test]
        public void GetOfflineChanges_ItemsNoItemsInTheCurrentRepository()
        {
            SynchItem newTree =
                new SynchItem();
            var listOfChanges = OfflineChangesDetector.GetOfflineChanges(newTree,@"backup.xml");
            Assert.IsNotNull(listOfChanges);
            Assert.AreEqual(listOfChanges.Count,listOfChanges.Count(c=>c.Value==OperationType.Delete));
            Assert.AreEqual(11,listOfChanges.Count);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetOfflineChanges_NoCurrentRepositorySent()
        {
            SynchItem newTree =null;
            OfflineChangesDetector.GetOfflineChanges(newTree,@"backup.xml");
        }

        [Test]
        [ExpectedException(typeof(PublisherSetupException))]
        public void GetOfflineChanges_WrongBackup()
        {
            SynchItem newTree =
                new SynchItem
                {
                    SynchItemData = new SynchItemData { Name = "newroot", Identifier = "newroot" },
                    Items = new List<SynchItem>{
                    new SynchItem{SynchItemData=new SynchItemData{Name="100",Identifier=@"newroot\100"},Items=new List<SynchItem>{
                        new SynchItem{SynchItemData=new SynchItemData{Name="110",Identifier=@"newroot\100\110"},Items=new List<SynchItem>{
                            new SynchItem{SynchItemData=new SynchItemData{Name="111",Identifier=@"newroot\100\110\111",Size=2}},
                            new SynchItem{SynchItemData=new SynchItemData{Name="112",Identifier=@"newroot\100\110\112"}}}},
                        new SynchItem{SynchItemData=new SynchItemData{Name="120",Identifier=@"newroot\100\120"},Items=new List<SynchItem>{
                            new SynchItem{SynchItemData=new SynchItemData{Name="123",Identifier=@"newroot\100\120\123"}}}}}},
                    new SynchItem{SynchItemData=new SynchItemData{Name="200",Identifier=@"newroot\200"}},
                    new SynchItem{SynchItemData=new SynchItemData{Name="300",Identifier=@"newroot\300"},Items=new List<SynchItem>{
                        new SynchItem{SynchItemData=new SynchItemData{Name="310",Identifier=@"newroot\300\310",Size=0}},
                        new SynchItem{SynchItemData=new SynchItemData{Name="320",Identifier=@"newroot\300\320"}},
                        new SynchItem{SynchItemData=new SynchItemData{Name="330",Identifier=@"newroot\300\330"}, Items=new List<SynchItem>{
                            new SynchItem{SynchItemData=new SynchItemData{Name="331",Identifier=@"newroot\300\330\331"},Items=new List<SynchItem>{
                                new SynchItem{SynchItemData=new SynchItemData{Name="331",Identifier=@"newroot\300\330\331\331"}}}},
                            new SynchItem{SynchItemData=new SynchItemData{Name="332", Identifier=@"newroot\300\330\332"}, Items= new List<SynchItem>{
                                new SynchItem{SynchItemData=new SynchItemData{Name="332", Identifier=@"newroot\300\330\332\332"}},
                                new SynchItem{SynchItemData=new SynchItemData{Name="332", Identifier=@"newroot\300\330\332\334"}},
                                new SynchItem{SynchItemData=new SynchItemData{Name="332", Identifier=@"newroot\300\330\332\335"}},
                                new SynchItem{SynchItemData=new SynchItemData{Name="333", Identifier=@"newroot\300\330\332\333",Size=34}}}}}}}}}
                };

            OfflineChangesDetector.GetOfflineChanges(newTree,@"backup.xml");
        }

        [Test]
        public void SaveBackup_OK()
        {
            PushPublisher pushPublisher = new PushPublisher();
            SynchItem newTree =
    new SynchItem
    {
        SynchItemData = new SynchItemData { Name = "newroot", Identifier = "newroot" },
        Items = new List<SynchItem>{
                    new SynchItem{SynchItemData=new SynchItemData{Name="100",Identifier=@"newroot\100"},Items=new List<SynchItem>{
                        new SynchItem{SynchItemData=new SynchItemData{Name="110",Identifier=@"newroot\100\110"},Items=new List<SynchItem>{
                            new SynchItem{SynchItemData=new SynchItemData{Name="111",Identifier=@"newroot\100\110\111",Size=2}},
                            new SynchItem{SynchItemData=new SynchItemData{Name="112",Identifier=@"newroot\100\110\112"}}}},
                        new SynchItem{SynchItemData=new SynchItemData{Name="120",Identifier=@"newroot\100\120"},Items=new List<SynchItem>{
                            new SynchItem{SynchItemData=new SynchItemData{Name="123",Identifier=@"newroot\100\120\123"}}}}}},
                    new SynchItem{SynchItemData=new SynchItemData{Name="200",Identifier=@"newroot\200"}},
                    new SynchItem{SynchItemData=new SynchItemData{Name="300",Identifier=@"newroot\300"},Items=new List<SynchItem>{
                        new SynchItem{SynchItemData=new SynchItemData{Name="310",Identifier=@"newroot\300\310",Size=0}},
                        new SynchItem{SynchItemData=new SynchItemData{Name="320",Identifier=@"newroot\300\320"}},
                        new SynchItem{SynchItemData=new SynchItemData{Name="330",Identifier=@"newroot\300\330"}, Items=new List<SynchItem>{
                            new SynchItem{SynchItemData=new SynchItemData{Name="331",Identifier=@"newroot\300\330\331"},Items=new List<SynchItem>{
                                new SynchItem{SynchItemData=new SynchItemData{Name="331",Identifier=@"newroot\300\330\331\331"}}}},
                            new SynchItem{SynchItemData=new SynchItemData{Name="332", Identifier=@"newroot\300\330\332"}, Items= new List<SynchItem>{
                                new SynchItem{SynchItemData=new SynchItemData{Name="332", Identifier=@"newroot\300\330\332\332"}},
                                new SynchItem{SynchItemData=new SynchItemData{Name="332", Identifier=@"newroot\300\330\332\334"}},
                                new SynchItem{SynchItemData=new SynchItemData{Name="332", Identifier=@"newroot\300\330\332\335"}},
                                new SynchItem{SynchItemData=new SynchItemData{Name="333", Identifier=@"newroot\300\330\332\333",Size=34}}}}}}}}}
    };


            pushPublisher.CurrentRepository = newTree;
            pushPublisher.Close("backupsaved.xml");
            var actual = Serializer.DeserializeFromFile<SynchItem>("backupsaved.xml");
            Assert.IsNotNull(actual);
            Assert.AreEqual(newTree.Items.Count,actual[0].Items.Count);

        }
    }
}
