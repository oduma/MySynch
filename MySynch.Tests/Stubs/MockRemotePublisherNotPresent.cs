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

        public void RemovePackage(ChangePushPackage packagePublished)
        {
            throw new NotImplementedException();
        }

        public SynchItem ListRepository()
        {
            throw new NotImplementedException();
        }

        public void InitiateUsingEndpoint(string endpointName)
        {
            return;
        }

        public RemoteResponse GetData(RemoteRequest remoteRequest)
        {
            throw new NotImplementedException();
        }
    }
}
