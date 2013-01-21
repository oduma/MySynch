﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySynch.Core.DataTypes;
using MySynch.Core.Interfaces;

namespace MySynch.Tests.Stubs
{
    public class ChangeApplyerNotPresent:IChangeApplyer
    {
        public string MachineName
        {
            get { throw new NotImplementedException(); }
        }

        public HeartbeatResponse GetHeartbeat()
        {
            return new HeartbeatResponse {Status = false};
        }

        public bool ApplyChangePackage(ChangePushPackage changePushPackage, string targetRootFolder, Func<string, string, bool> copyMethod)
        {
            throw new NotImplementedException();
        }
    }
}