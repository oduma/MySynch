using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySynch.Contracts;
using MySynch.Contracts.Messages;

namespace MySynch.Core
{
    public class LocalSourceOfData:ISourceOfData
    {
        public RemoteResponse GetData(RemoteRequest remoteRequest)
        {
            return null;
        }

        public HeartbeatResponse GetHeartbeat()
        {
            return new HeartbeatResponse {Status = true};
        }
    }
}
