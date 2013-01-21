using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySynch.Core.DataTypes;
using MySynch.Core.Interfaces;

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
