using System;
using System.Collections.Generic;
using MySynch.Contracts.Messages;
using MySynch.Proxies;
using MySynch.Proxies.Interfaces;

namespace MySynch.Tests.Stubs
{
    public class MockRemoteSubscriber:ISubscriberProxy
    {
        private string targetRootFolder;

        public GetHeartbeatResponse GetHeartbeat()
        {
            return new GetHeartbeatResponse {Status = true};
        }

        public ApplyChangePackageResponse ApplyChangePackage(PublishPackageRequestResponse publishPackageRequestResponse)
        {
            if(targetRootFolder=="wrong folder")
                throw new Exception();
            bool result = true;

            foreach (ChangePushItem upsert in publishPackageRequestResponse.ChangePushItems)
            {
                var tempResult = copyMethod(upsert.AbsolutePath,
                                             upsert.AbsolutePath.Replace(publishPackageRequestResponse.SourceRootName, targetRootFolder));
                result = result && tempResult;
            }
            return new ApplyChangePackageResponse {Status = result};
        }

        public TryOpenChannelResponse TryOpenChannel(TryOpenChannelRequest sourceOfDataEndpointName)
        {
            return new TryOpenChannelResponse {Status = true};
        }

        private bool copyMethod(string absolutePath, string replace)
        {
            return true;
        }

        public void InitiateUsingPort(int port)
        {
            return;
        }
    }
}
