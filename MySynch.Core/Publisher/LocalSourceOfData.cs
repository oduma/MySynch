using MySynch.Contracts;
using MySynch.Contracts.Messages;

namespace MySynch.Core.Publisher
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
