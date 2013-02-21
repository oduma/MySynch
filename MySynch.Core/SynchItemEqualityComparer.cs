using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySynch.Contracts.Messages;
using MySynch.Core.DataTypes;

namespace MySynch.Core
{
    internal class SynchItemEqualityComparer:IEqualityComparer<SynchItem>
    {
        public bool Equals(SynchItem x, SynchItem y)
        {
            if (x == null || string.IsNullOrEmpty(x.Identifier))
                return false;
            if (y == null || string.IsNullOrEmpty(y.Identifier))
                return false;
            return x.Identifier.Equals(y.Identifier);
        }

        public int GetHashCode(SynchItem obj)
        {
            return obj.Identifier.GetHashCode();
        }
    }
}
