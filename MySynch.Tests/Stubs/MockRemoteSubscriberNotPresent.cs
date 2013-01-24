using System;
using MySynch.Contracts.Messages;
using MySynch.Proxies;

namespace MySynch.Tests.Stubs
{
    public class MockRemoteSubscriberNotPresent:ISubscriberProxy
    {
        public HeartbeatResponse GetHeartbeat()
        {
            return new HeartbeatResponse {Status = false};
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
