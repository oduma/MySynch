using System;
using MySynch.Contracts.Messages;
using MySynch.Proxies;

namespace MySynch.Tests.Stubs
{
    public class MockRemoteSubscriber:ISubscriberProxy
    {
        private string targetRootFolder;

        public HeartbeatResponse GetHeartbeat()
        {
            return new HeartbeatResponse {Status = true};
        }

        public bool ApplyChangePackage(ChangePushPackage changePushPackage, string dataSourceEndpointName)
        {
            if(targetRootFolder=="wrong folder")
                throw new Exception();
            bool result = true;

            foreach (ChangePushItem upsert in changePushPackage.ChangePushItems)
            {
                var tempResult = copyMethod(upsert.AbsolutePath,
                                             upsert.AbsolutePath.Replace(changePushPackage.SourceRootName, targetRootFolder));
                result = result && tempResult;
            }
            return result;
        }

        public string GetTargetRootFolder()
        {
            return targetRootFolder;
        }

        private bool copyMethod(string absolutePath, string replace)
        {
            return true;
        }

        public void InitiateUsingEndpoint(string endpointName)
        {
            return;
        }
    }
}
