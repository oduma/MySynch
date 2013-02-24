using System.Collections.Generic;
using System.Linq;
using MySynch.Contracts.Messages;
using MySynch.Core;
using MySynch.Core.DataTypes;
using NUnit.Framework;

namespace MySynch.Tests
{
    [TestFixture]
    public class ChangePublisherGetOfflineChangesTests
    {

        [Test]
        public void GetOfflineChanges_ItemsToBeInserted()
        {
            ChangePublisher changePublisher = new ChangePublisher();
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
            var listOfChanges = changePublisher.GetOfflineChanges(newTree);
            Assert.IsNotNull(listOfChanges);
            Assert.AreEqual(listOfChanges.Count, listOfChanges.Count(c => c.Value == OperationType.Insert));
            Assert.AreEqual(8, listOfChanges.Count);

        }

        [Test]
        public void GetOfflineChanges_ItemsToBeDeleted()
        {
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
            var mockItemDiscoverer = MockTestHelper.MockItemDiscoverer("generate backup");
            changePublisher.Initialize("generate backup", mockItemDiscoverer);
            changePublisher.SaveSettingsEndExit();
        }

    }
}
