using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySynch.Common;
using MySynch.Contracts;
using MySynch.Contracts.Messages;

namespace MySynch.Core
{
    public class RemoteSourceOfData:ISourceOfData
    {
        public RemoteResponse GetData(RemoteRequest remoteRequest)
        {
            LoggingManager.Debug("Using remote datasource returning contents of file: " + remoteRequest.FileName); 
            RemoteResponse response = new RemoteResponse();
            return response;
        }

        public HeartbeatResponse GetHeartbeat()
        {
            return new HeartbeatResponse {Status = true};
        }
    }
}
