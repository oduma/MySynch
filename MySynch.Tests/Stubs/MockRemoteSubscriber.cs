using System;
using MySynch.Contracts;
using MySynch.Contracts.Messages;
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
        public ApplyChangePushItemResponse ApplyChangePushItem(ApplyChangePushItemRequest applyChangePushItemRequest)
        {
            if (targetRootFolder == "wrong folder")
                throw new Exception();
            return new ApplyChangePushItemResponse
                       {
                           ChangePushItem =
                               new ChangePushItem
                                   {
                                       AbsolutePath =
                                           applyChangePushItemRequest.ChangePushItem.AbsolutePath.Replace(
                                               applyChangePushItemRequest.SourceRootName, targetRootFolder)
                                   },
                           Success = copyMethod("abc", "def")
                       };
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

        public void DestroyAtPort(int port)
        {
            return;
        }

        public void InitiateDuplexUsingPort<TCallback>(TCallback callbackInstance, int port)
        {
            return;
        }
    }
}
