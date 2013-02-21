using System;
using System.Collections.Generic;
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

        public bool ApplyChangePackage(ChangePushPackage changePushPackage)
        {
            throw new NotImplementedException();
        }

        public string GetTargetRootFolder()
        {
            throw new NotImplementedException();
        }

        public bool TryOpenChannel(string sourceOfDataEndpointName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ChangePushPackage> GetDifferenceAsMessages(SynchItem currentPublisherRepository)
        {
            throw new NotImplementedException();
        }

        public void InitiateUsingEndpoint(string endpointName)
        {
            return;
        }
    }
}
