using System;
using System.Collections.Generic;
using MySynch.Contracts;
using MySynch.Contracts.Messages;

namespace MySynch.Tests.Stubs
{
    public class ChangeApplyerNotPresent:ISubscriber
    {
        
        public GetHeartbeatResponse GetHeartbeat()
        {
            return new GetHeartbeatResponse {Status = false};
        }

        public ReceiveMessageResponse ReceiveMessage(ReceiveMessageRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
