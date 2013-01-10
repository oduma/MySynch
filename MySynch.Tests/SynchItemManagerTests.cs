using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MySynch.Core;
using MySynch.Core.Interfaces;
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

        

        [SetUp]
        public void SetUp()
        {
            _initialLoad = new List<SynchItem> {
                new SynchItem{Name="root",Identifier="root",Items=new List<SynchItem>{
                    new SynchItem{Name="100",Identifier=@"root\100",Items=new List<SynchItem>{
                        new SynchItem{Name="110",Identifier=@"root\100\110",Items=new List<SynchItem>{
                            new SynchItem{Name="111",Identifier=@"root\100\110\111"},
                            new SynchItem{Name="112",Identifier=@"root\100\110\112"}}},
                        new SynchItem{Name="120",Identifier=@"root\100\120",Items=new List<SynchItem>{
                            new SynchItem{Name="121",Identifier=@"root\100\120\121"},
                            new SynchItem{Name="122",Identifier=@"root\100\120\122"},
                            new SynchItem{Name="123",Identifier=@"root\100\120\123"}}}}},
                    new SynchItem{Name="200",Identifier=@"root\200"},
                    new SynchItem{Name="300",Identifier=@"root\300",Items=new List<SynchItem>{
                        new SynchItem{Name="310",Identifier=@"root\300\310"},
                        new SynchItem{Name="320",Identifier=@"root\300\320"},
                        new SynchItem{Name="330",Identifier=@"root\300\330", Items=new List<SynchItem>{
                            new SynchItem{Name="331",Identifier=@"root\300\330\331",Items=new List<SynchItem>{
                                new SynchItem{Name="331",Identifier=@"root\300\330\331\331"}}},
                            new SynchItem{Name="332", Identifier=@"root\300\330\332", Items= new List<SynchItem>{
                                new SynchItem{Name="332", Identifier=@"root\300\330\332\332"},
                                new SynchItem{Name="333", Identifier=@"root\300\330\332\333"}}}}}}}}}};
        }

        [Test]
        public void ListAllItems_Ok()
        {
            SynchItemManager sim = new SynchItemManager(_initialLoad);

            var actualItems = sim.ListAllItems();

            Assert.AreEqual(_initialLoad, actualItems);
        }

        [Test]
        public void ListAllItems_NoItem()
        {
            SynchItemManager sim = new SynchItemManager(null);

            Assert.IsNotNull( sim.ListAllItems());
            Assert.IsEmpty(sim.ListAllItems());
        }

        [Test]
        public void ListItems_Ok()
        {
            SynchItemManager sim = new SynchItemManager(_initialLoad);

            var actualItems = sim.ListItems(@"root\100");

            Assert.AreEqual(_initialLoad[0].Items[0].Items, actualItems);

            actualItems = sim.ListItems(@"root\300\330\331");
            Assert.AreEqual(_initialLoad[0].Items[2].Items[2].Items[0].Items, actualItems);
        }

        [Test]
        public void ListItems_NoItems_UnderParent()
        {
            SynchItemManager sim = new SynchItemManager(_initialLoad);

            var actualItems = sim.ListItems(@"root\200");

            Assert.AreEqual(0,actualItems.Count);

            actualItems = sim.ListItems(@"root\300\330\331\331");
            Assert.AreEqual(0, actualItems.Count);
        }

        [Test]
        public void ListItems_NoParentItem()
        {
            SynchItemManager sim = new SynchItemManager(_initialLoad);

            var actualItems = sim.ListItems(@"root\210");

            Assert.IsNull(actualItems);

            actualItems = sim.ListItems(@"root\300\330\331\331\331");
            Assert.IsNull(actualItems);
        }

        public void InsertItems_Parent_Empty()
        {
            Assert.Fail();
        }

        public void InsertItems_Parent_NotEmpty()
        {
            Assert.Fail();
        }

        public void InsertItems_Parent_NotPresent()
        {
            Assert.Fail();
        }

        public void InsertItems_NoItems()
        {
            Assert.Fail();
        }

        public void UpdateItem_Ok()
        {
            Assert.Fail();
        }

        public void UpdateItem_NoParent()
        {
            Assert.Fail();
        }

        public void UpdateItem_NoItem()
        {
            Assert.Fail();
        }

        public void RemoveItem_Ok()
        {
            Assert.Fail();
        }

        public void RemoveItem_NoItem()
        {
            Assert.Fail();
        }

        public void RemoveItem_Item_Has_SubItems()
        {
            Assert.Fail();
        }

        public void RemoveItems_Ok()
        {
            Assert.Fail();
        }

        public void RemoveItems_NoParent()
        {
            Assert.Fail();
        }

        public void RemoveItems_ParentEmpty()
        {
            Assert.Fail();
        }
    }
}
