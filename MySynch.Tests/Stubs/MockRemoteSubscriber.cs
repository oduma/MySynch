using System;
using MySynch.Contracts.Messages;
using MySynch.Proxies;

namespace MySynch.Tests.Stubs
{
    public class MockRemoteSubscriber:ISubscriberProxy
    {
        public HeartbeatResponse GetHeartbeat()
        {
            return new HeartbeatResponse {Status = true};
        }

        public bool ApplyChangePackage(ChangePushPackage changePushPackage, string targetRootFolder, Func<string, string, bool> copyMethod)
        {
            throw new NotImplementedException();
        }

        public void InitiateUsingEndpoint(string endpointName)
        {
            return;
        }
    }
}
