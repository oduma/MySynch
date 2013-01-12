using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using MySynch.Core.DataTypes;

namespace MySynch.Utils
{
    class Program
    {
        static void Main(string[] args)
        {
            List<SynchItem> _initialLoad = new List<SynchItem> {
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

            XmlSerializer xmlSerializer = new XmlSerializer(typeof (SynchItem));

            using (FileStream fs = new FileStream("items.xml", FileMode.CreateNew))
                xmlSerializer.Serialize(fs,_initialLoad[0]);
        }
    }
}
