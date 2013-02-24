using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySynch.Core.DataTypes;

namespace MySynch.Core
{
    internal class SynchItemDataEqualityComparer : IEqualityComparer<SynchItemData>
    {
        public bool Equals(SynchItemData x, SynchItemData y)
        {
            if (x == null || string.IsNullOrEmpty(x.Identifier))
                return false;
            if (y == null || string.IsNullOrEmpty(y.Identifier))
                return false;
            return x.Identifier.Equals(y.Identifier);
        }

        public int GetHashCode(SynchItemData obj)
        {
            return obj.Identifier.GetHashCode();
        }
    }
}
