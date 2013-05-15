using System.Collections.Generic;
using System.Linq;
using MySynch.Core.Publisher;
using NUnit.Framework;
using MySynch.Core.DataTypes;

namespace MySynch.Tests
{
    [TestFixture]
    public class SynchItemManagerTests
    {
        /// <summary>
        /// Loaded like this
        /// root
        ///     100
        ///         110
        ///             111
        ///             112
        ///         120
        ///             121
        ///             122
        ///             123
        ///     200
        ///     300
        ///         310
        ///         320
        ///         330
        ///             331
        ///                 331
        ///             332
        ///                 332
        ///                 333
        /// </summary>
        private List<SynchItem> _initialLoad;

        /// <summary>
        /// root\200\
        ///     210
        ///         211
        ///         212
        ///     220
        ///         220
        ///             221
        /// </summary>
        private List<SynchItem> _additionalLoad;

        [SetUp]
        public void SetUp()
        {
            _initialLoad = new List<SynchItem> {
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
                                new SynchItem{SynchItemData=new SynchItemData{Name="333", Identifier=@"root\300\330\332\333"}}}}}}}}}}};

            _additionalLoad = new List<SynchItem>{
                new SynchItem{SynchItemData=new SynchItemData{Identifier=@"root\200\210", Name="210"},Items=new List<SynchItem>{
                    new SynchItem{SynchItemData= new SynchItemData{Identifier=@"root\200\210\211",Name="211"}},
                    new SynchItem {SynchItemData=new SynchItemData{Identifier=@"root\200\210\212",Name="212"}}}},
                new SynchItem{SynchItemData=new SynchItemData{Identifier=@"root\200\220", Name="220"},Items=new List<SynchItem>{
                    new SynchItem{SynchItemData=new SynchItemData{Identifier=@"root\200\220\220",Name="220"},Items= new List<SynchItem>{
                        new SynchItem{SynchItemData=new SynchItemData{Identifier=@"root\200\220\220\221", Name="221"}}
                    }}
                }}};
        }
        #region static methods tests
        [Test]
        public void GetItem_Ok()
        {
            Assert.AreEqual(@"root\300\330\331", SynchItemManager.GetItemLowestAvailableParrent(_initialLoad[0], @"root\300\330\331").SynchItemData.Identifier);
        }
        [Test]
        public void GetItem_Parent_Found_Ok()
        {
            Assert.AreEqual(@"root\300\330\331", SynchItemManager.GetItemLowestAvailableParrent(_initialLoad[0], @"root\300\330\331\332").SynchItemData.Identifier);
        }
        [Test]
        public void GetItem_GrandParent_Found_Ok()
        {
            Assert.AreEqual(@"root\300\330", SynchItemManager.GetItemLowestAvailableParrent(_initialLoad[0], @"root\300\330\333\332").SynchItemData.Identifier);
        }
        [Test]
        public void UpdateItem_ItemExists()
        {
            Assert.AreEqual(1,_initialLoad[0].Items[2].Items[0].SynchItemData.Size);
            SynchItemManager.UpdateExistingItem(_initialLoad[0], @"root\300\310",2);
            Assert.AreEqual(@"root\300\310", _initialLoad[0].Items[2].Items[0].SynchItemData.Identifier);
            Assert.AreEqual(2,_initialLoad[0].Items[2].Items[0].SynchItemData.Size);
        }

        [Test]
        public void UpdateItem_ItemDoesNotExist()
        {
            SynchItemManager.UpdateExistingItem(_initialLoad[0], @"root\300\340\341\341\342",3);
            Assert.AreEqual(@"root\300\340", _initialLoad[0].Items[2].Items[3].SynchItemData.Identifier);
            Assert.AreEqual(@"root\300\340\341", _initialLoad[0].Items[2].Items[3].Items[0].SynchItemData.Identifier);
            Assert.AreEqual(@"root\300\340\341\341", _initialLoad[0].Items[2].Items[3].Items[0].Items[0].SynchItemData.Identifier);
            Assert.AreEqual(@"root\300\340\341\341\342", _initialLoad[0].Items[2].Items[3].Items[0].Items[0].Items[0].SynchItemData.Identifier);
        }
        [Test]
        public void UpdateItem_NoItemSent()
        {
            SynchItemManager.UpdateExistingItem(_initialLoad[0], null,0);
        }
        [Test]
        public void UpdateItem_NoTopItemSent()
        {
            SynchItemManager.UpdateExistingItem(null, @"root\300",100);
        }

        [Test]
        public void DeleteItem_ItemExists()
        {
            SynchItemManager.DeleteItem(_initialLoad[0], @"root\300\330");
            Assert.IsNull(_initialLoad[0].Items[2].Items.FirstOrDefault(i => i.SynchItemData.Identifier == @"root\300\330"));
            Assert.IsNotNull(_initialLoad[0].Items[2].Items.FirstOrDefault(i => i.SynchItemData.Identifier == @"root\300\310"));
            Assert.IsNotNull(_initialLoad[0].Items[2].Items.FirstOrDefault(i => i.SynchItemData.Identifier == @"root\300\320"));
        }

        [Test]
        public void DeleteItem_ItemDoesNotExist()
        {
            SynchItemManager.DeleteItem(_initialLoad[0], @"root\300\340\341\341\342");
        }
        [Test]
        public void DeleteItem_NoItemSent()
        {
            SynchItemManager.DeleteItem(_initialLoad[0], null);
        }
        [Test]
        public void DeleteItem_NoTopItemSent()
        {
            SynchItemManager.DeleteItem(null, @"root\300");
        }
        [Test]
        public void AddNewItem_ParentItemExists()
        {
            SynchItemManager.AddItem(_initialLoad[0], @"root\300\340",2);
            Assert.AreEqual(@"root\300\340", _initialLoad[0].Items[2].Items[3].SynchItemData.Identifier);
            Assert.AreEqual(2, _initialLoad[0].Items[2].Items[3].SynchItemData.Size);
        }

        [Test]
        public void AddNewItem_ParentItemDoesNotExist()
        {
            SynchItemManager.AddItem(_initialLoad[0], @"root\300\340\341\341\342",3);
            Assert.AreEqual(@"root\300\340", _initialLoad[0].Items[2].Items[3].SynchItemData.Identifier);
            Assert.AreEqual(@"root\300\340\341", _initialLoad[0].Items[2].Items[3].Items[0].SynchItemData.Identifier);
            Assert.AreEqual(@"root\300\340\341\341", _initialLoad[0].Items[2].Items[3].Items[0].Items[0].SynchItemData.Identifier);
            Assert.AreEqual(@"root\300\340\341\341\342", _initialLoad[0].Items[2].Items[3].Items[0].Items[0].Items[0].SynchItemData.Identifier);
            Assert.AreEqual(0, _initialLoad[0].Items[2].Items[3].SynchItemData.Size);
            Assert.AreEqual(0, _initialLoad[0].Items[2].Items[3].Items[0].SynchItemData.Size);
            Assert.AreEqual(0, _initialLoad[0].Items[2].Items[3].Items[0].Items[0].SynchItemData.Size);
            Assert.AreEqual(3, _initialLoad[0].Items[2].Items[3].Items[0].Items[0].Items[0].SynchItemData.Size);
        }

        [Test]
        public void AddNewItem_ItemAlreadyExists()
        {
            SynchItemManager.AddItem(_initialLoad[0], @"root\300",0);
            Assert.AreEqual(@"root\300", _initialLoad[0].Items[2].SynchItemData.Identifier);
            Assert.AreEqual(3, _initialLoad[0].Items[2].Items.Count);
        }
        [Test]
        public void AddNewItem_NoItemSent()
        {
            SynchItemManager.AddItem(_initialLoad[0], null,0);
        }
        [Test]
        public void AddNewItem_NoTopItemSent()
        {
            SynchItemManager.AddItem(null, @"root\300",0);
        }

        [Test]
        public void FlattenTree_Ok()
        {
            var listOfItems = SynchItemManager.FlattenTree(_initialLoad[0]);
            Assert.IsNotNull(listOfItems);
        }

        #endregion
    }
}
