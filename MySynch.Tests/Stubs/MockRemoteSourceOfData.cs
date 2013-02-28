using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySynch.Contracts.Messages;
using MySynch.Proxies;
using MySynch.Proxies.Interfaces;

namespace MySynch.Tests.Stubs
{
    public class MockRemoteSourceOfData:ISourceOfDataProxy
    {
        public GetHeartbeatResponse GetHeartbeat()
        {
            return new GetHeartbeatResponse {Status = true};
        }

        public RemoteResponse GetData(RemoteRequest remoteRequest)
        {
            throw new NotImplementedException();
        }

        public void InitiateUsingEndpoint(string endpointName)
        {
            return;
        }
    }
}
