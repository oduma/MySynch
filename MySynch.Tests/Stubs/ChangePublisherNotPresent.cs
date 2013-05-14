using System;
using MySynch.Contracts;
using MySynch.Contracts.Messages;

namespace MySynch.Tests.Stubs
{
    public class ChangePublisherNotPresent:IPublisher
    {
        public GetHeartbeatResponse GetHeartbeat()
        {
            return new GetHeartbeatResponse { Status = false };
        }

        public GetDataResponse GetData(GetDataRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
