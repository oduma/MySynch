using System;
using MySynch.Contracts.Messages;
using MySynch.Proxies;

namespace MySynch.Tests.Stubs
{
    public class MockRemotePublisher:IPublisherProxy
    {
        public HeartbeatResponse GetHeartbeat()
        {
            return new HeartbeatResponse {Status = true};
        }

        public ChangePushPackage PublishPackage()
        {
            throw new NotImplementedException();
        }

        public void RemovePackage(ChangePushPackage packagePublished)
        {
            throw new NotImplementedException();
        }
        public void InitiateUsingEndpoint(string endpointName)
        {
            return;
        }
    }
}
