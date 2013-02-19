﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySynch.Contracts;
using MySynch.Contracts.Messages;
using MySynch.Core.DataTypes;
using MySynch.Core.Interfaces;

namespace MySynch.Tests.Stubs
{
    public class ChangeApplyerNotPresent:ISubscriber
    {
        public string MachineName
        {
            get { throw new NotImplementedException(); }
        }

        public HeartbeatResponse GetHeartbeat()
        {
            return new HeartbeatResponse {Status = false};
        }

        public bool ApplyChangePackage(ChangePushPackage changePushPackage)
        {
            throw new NotImplementedException();
        }

        public string GetTargetRootFolder()
        {
            throw new NotImplementedException();
        }

        public bool TryOpenChannel(string sourceOfDataEndpointName)
        {
            throw new NotImplementedException();
        }
    }
}
