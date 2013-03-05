using System;
using MySynch.Contracts.Messages;
using MySynch.Proxies.Interfaces;

namespace MySynch.Tests.Stubs
{
    public class MockRemotePublisher:IPublisherProxy
    {
        public GetHeartbeatResponse GetHeartbeat()
        {
            return new GetHeartbeatResponse {Status = true};
        }

        public PublishPackageRequestResponse PublishPackage()
        {
            throw new NotImplementedException();
        }

        public void RemovePackage(PublishPackageRequestResponse packageRequestResponsePublished)
        {
            throw new NotImplementedException();
        }

        public void InitiateUsingPort(int port)
        {
            return;
        }
    }
}
