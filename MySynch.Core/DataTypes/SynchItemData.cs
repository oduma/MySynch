using System;

namespace MySynch.Core.DataTypes
{
    public class SynchItemData
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

        public long Size { get; set; }

        public DateTime WriteDate { get; set; }
    }
}
