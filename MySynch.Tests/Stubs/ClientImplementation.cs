﻿using System;
using System.Collections.Generic;
using System.ServiceModel.Description;
using MySynch.Common.WCF.Clients;
using MySynch.Contracts.Messages;

namespace MySynch.Tests.Stubs
{
    internal class ClientImplementation:BaseClient<ITest1>,ITest1Proxy
    {
        public void Opertaion1()
        {
            throw new NotImplementedException();
        }

        public GetHeartbeatResponse GetHeartbeat()
        {
            throw new NotImplementedException();
        }

        protected override List<IEndpointBehavior> GetEndpointBehaviors()
        {
            return new List<IEndpointBehavior>();
        }
    }
}
