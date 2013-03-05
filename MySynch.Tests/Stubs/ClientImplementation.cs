using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySynch.Contracts.Messages;
using MySynch.Core.WCF.Clients;

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
    }
}
