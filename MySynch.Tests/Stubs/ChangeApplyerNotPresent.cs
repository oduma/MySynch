using System;
using System.Collections.Generic;
using MySynch.Contracts;
using MySynch.Contracts.Messages;

namespace MySynch.Tests.Stubs
{
    public class ChangeApplyerNotPresent:ISubscriber
    {
        public string MachineName
        {
            get { throw new NotImplementedException(); }
        }

        public GetHeartbeatResponse GetHeartbeat()
        {
            return new GetHeartbeatResponse {Status = false};
        }

        public ApplyChangePackageResponse ApplyChangePackage(PublishPackageRequestResponse publishPackageRequestResponse)
        {
            throw new NotImplementedException();
        }

        public TryOpenChannelResponse TryOpenChannel(TryOpenChannelRequest sourceOfDataEndpointName)
        {
            throw new NotImplementedException();
        }
    }
}
