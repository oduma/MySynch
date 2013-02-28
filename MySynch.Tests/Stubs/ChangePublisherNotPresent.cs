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

        public GetHeartbeatResponse GetHeartbeat()
        {
            return new GetHeartbeatResponse { Status = false };
        }

        public PublishPackageRequestResponse PublishPackage()
        {
            throw new NotImplementedException();
        }

        public void RemovePackage(PublishPackageRequestResponse packageRequestResponsePublished)
        {
            throw new NotImplementedException();
        }
    }
}
