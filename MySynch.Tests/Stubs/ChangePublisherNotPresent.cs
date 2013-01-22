using System;
using MySynch.Contracts;
using MySynch.Contracts.Messages;

namespace MySynch.Tests.Stubs
{
    public class ChangePublisherNotPresent:IPublisher
    {
        public string MachineName
        {
            get { throw new NotImplementedException(); }
        }

        public HeartbeatResponse GetHeartbeat()
        {
            return new HeartbeatResponse { Status = false };
        }

        public ChangePushPackage PublishPackage()
        {
            throw new NotImplementedException();
        }
    }
}
