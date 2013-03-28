using System;
using System.Collections.Generic;
using MySynch.Contracts;
using MySynch.Contracts.Messages;
using MySynch.Proxies;
using MySynch.Proxies.Interfaces;

namespace MySynch.Tests.Stubs
{
    internal class MockRemoteSubscriberNotPresent:ISubscriberProxy
    {
        public GetHeartbeatResponse GetHeartbeat()
        {
            return new GetHeartbeatResponse {Status = false};
        }

        public void ConsumePackage(PublishPackageRequestResponse publishPackageRequestResponse)
        {
            throw new NotImplementedException();
        }

        public TryOpenChannelResponse TryOpenChannel(TryOpenChannelRequest sourceOfDataEndpointName)
        {
            throw new NotImplementedException();
        }

        public void ForceSetTheSubscriberFeedback(ISubscriberFeedback SubscriberFeedback)
        {
            return;
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
