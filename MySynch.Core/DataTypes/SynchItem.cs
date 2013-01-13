using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MySynch.Core.DataTypes
{
    public class SynchItem
    {
        public string Name
        {
            get;
            set;
        }

        public string Identifier
        {
            get;
            set;
        }

        public string RelativeName { get; set; }
        public List<SynchItem> Items
        {
            get;
            set;
        }
    }
}
