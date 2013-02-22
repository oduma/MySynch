using System.Collections.Generic;

namespace MySynch.Core.DataTypes
{
    public class SynchItem
    {
        public SynchItemData SynchItemData { get; set; }

        public List<SynchItem> Items
        {
            get;
            set;
        }
    }
}
