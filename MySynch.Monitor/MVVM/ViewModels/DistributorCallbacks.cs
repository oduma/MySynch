using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySynch.Contracts;
using MySynch.Contracts.Messages;

namespace MySynch.Monitor.MVVM.ViewModels
{
    public class DistributorCallbacks:IDistributorCallbacks
    {
        public void PackagePublished(ChangePushPackage changePushPackage)
        {
            throw new NotImplementedException();
        }

        public void PackageApplyed(ChangePushPackage changePushPackage)
        {
            throw new NotImplementedException();
        }
    }
}
