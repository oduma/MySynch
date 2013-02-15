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

        public bool ApplyChangePackage(ChangePushPackage changePushPackage,string dataSourceEnpointName)
        {
            throw new NotImplementedException();
        }

        public string GetTargetRootFolder()
        {
            throw new NotImplementedException();
        }

        public void InitiateUsingEndpoint(string endpointName)
        {
            return;
        }
    }
}
