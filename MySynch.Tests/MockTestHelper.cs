using System.Collections.Generic;
using Moq;
using MySynch.Core.DataTypes;
using MySynch.Core.Interfaces;
using MySynch.Core.Publisher;

namespace MySynch.Tests
{
    internal static class MockTestHelper
    {

        internal static ItemDiscoverer MockItemDiscoverer(string folderPath)
        {
            var mockItemDiscoverer = new Mock<ItemDiscoverer>();
            if (folderPath == "generate backup")
                mockItemDiscoverer.Setup(m => m.DiscoverFromFolder(folderPath)).Returns(
                new SynchItem
                {
                    SynchItemData = new SynchItemData { Name = "root", Identifier = "root" },
                    Items = new List<SynchItem>{
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
                                new SynchItem{SynchItemData=new SynchItemData{Name="333", Identifier=@"root\300\330\332\333"}}}}}}}}}
                });
            else
                mockItemDiscoverer.Setup(m => m.DiscoverFromFolder(folderPath)).Returns(new SynchItem());
            return mockItemDiscoverer.Object;
        }


    }
}
