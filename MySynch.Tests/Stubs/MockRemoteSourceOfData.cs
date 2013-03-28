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

        public void InitiateUsingPort(int port)
        {
            return;
        }

        public void DestroyAtPort(int port)
        {
            return;
        }

        public void InitiateDuplexUsingPort<TCallback>(TCallback callbackInstance, int port)
        {
            return;
        }
    }
}
