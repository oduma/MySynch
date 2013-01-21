using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MySynch.Core.DataTypes
{
    public enum Status
    {
        NotChecked=0,
        Ok,
        OfflineTemporary,
        OfflinePermanent
    }
}
