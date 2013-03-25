using System;
using MySynch.Contracts.Messages;
using MySynch.Proxies;
using MySynch.Proxies.Interfaces;

namespace MySynch.Tests.Stubs
{
    public class MockRemotePublisherNotPresent:IPublisherProxy
    {
        public GetHeartbeatResponse GetHeartbeat()
        {
            return new GetHeartbeatResponse {Status = false};
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

        public void DestroyAtPort(int port)
        {
            return;
        }
    }
}
