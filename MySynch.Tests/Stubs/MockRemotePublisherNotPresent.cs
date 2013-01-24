using System;
using MySynch.Contracts.Messages;
using MySynch.Proxies;

namespace MySynch.Tests.Stubs
{
    public class MockRemotePublisherNotPresent:IPublisherProxy
    {
        public HeartbeatResponse GetHeartbeat()
        {
            return new HeartbeatResponse {Status = false};
        }

        public ChangePushPackage PublishPackage()
        {
            throw new NotImplementedException();
        }

        public void InitiateUsingEndpoint(string endpointName)
        {
            return;
        }
    }
}
