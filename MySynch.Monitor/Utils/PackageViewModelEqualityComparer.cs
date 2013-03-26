using System;
using System.Collections.Generic;
using MySynch.Monitor.MVVM.ViewModels;

namespace MySynch.Monitor.Utils
{
    internal class PackageViewModelEqualityComparer:IEqualityComparer<PackageViewModel>
    {
        public bool Equals(PackageViewModel x, PackageViewModel y)
        {
            if (x == null || y == null || x.PackageId == Guid.Empty || y.PackageId == Guid.Empty)
                return false;
            return (x.PackageId == y.PackageId);
        }

        public int GetHashCode(PackageViewModel obj)
        {
            return obj.GetHashCode();
        }
    }
}
