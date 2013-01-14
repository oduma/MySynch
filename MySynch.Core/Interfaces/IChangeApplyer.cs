using System;
using MySynch.Core.DataTypes;

namespace MySynch.Core.Interfaces
{
    public interface IChangeApplyer
    {
        bool ApplyChangePackage(ChangePushPackage changePushPackage, string targetRootFolder,Func<string,string,bool> copyMethod);
    }
}
