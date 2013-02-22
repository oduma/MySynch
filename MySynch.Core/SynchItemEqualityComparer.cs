using System.Collections.Generic;
using MySynch.Core.DataTypes;

namespace MySynch.Core
{
    internal class SynchItemEqualityComparer:IEqualityComparer<SynchItem>
    {
        public bool Equals(SynchItem x, SynchItem y)
        {
            if (x == null || x.SynchItemData==null || string.IsNullOrEmpty(x.SynchItemData.Identifier))
                return false;
            if (y == null || y.SynchItemData==null || string.IsNullOrEmpty(y.SynchItemData.Identifier))
                return false;
            return x.SynchItemData.Identifier.Equals(y.SynchItemData.Identifier);
        }

        public int GetHashCode(SynchItem obj)
        {
            return obj.SynchItemData.Identifier.GetHashCode();
        }
    }
}
