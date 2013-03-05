using System;
using System.Collections.Generic;
using System.IO;
using MySynch.Contracts.Messages;
using MySynch.Proxies;
using MySynch.Proxies.Interfaces;
using NUnit.Framework;

namespace MySynch.Tests.Integration
{
    [TestFixture]
    [Category("integration")]
    [Ignore("Start the servives at the specified address and after that this should run")]
    public class SubscriberTests
    {
        [Test]
        public void SubscriberUpAndAccessible()
        {
            ISubscriberProxy subscriberProxy = new SubscriberClient();
            subscriberProxy.InitiateUsingPort(8765);
            Assert.True(subscriberProxy.TryOpenChannel(new TryOpenChannelRequest { SourceOfDataPort = 8765 }).Status);
            var result = subscriberProxy.ApplyChangePackage(new PublishPackageRequestResponse());
            Assert.False(result.Status);

        }
        [Test]
        [Ignore(@"Requires Subscriber service to be defined on the root folder: C:\MySynch.Dest.Test.Root\")]
        public void SubscriberApplyChanges_Ok()
        {
            if (File.Exists(@"C:\MySynch.Dest.Test.Root\File1.xml"))
                File.Delete(@"C:\MySynch.Dest.Test.Root\File1.xml");
            ISubscriberProxy subscriberProxy = new SubscriberClient();
            subscriberProxy.InitiateUsingPort(8765);
            PublishPackageRequestResponse publishedPackageRequestResponse = new PublishPackageRequestResponse
            {
                PackageId = Guid.NewGuid(),
                Source = "SCIENDO-LAPTOP",
                SourceRootName = @"C:\MySynch.Source.Test.Root\",
                ChangePushItems =
                    new List<ChangePushItem>
                                                                 {
                                                                     new ChangePushItem
                                                                         {
                                                                             AbsolutePath =
                                                                                 @"C:\MySynch.Source.Test.Root\File1.xml",
                                                                             OperationType = OperationType.Insert
                                                                         }
                                                                 }
            };
            Assert.True(subscriberProxy.TryOpenChannel(new TryOpenChannelRequest { SourceOfDataPort = 8765 }).Status);
            var result = subscriberProxy.ApplyChangePackage(publishedPackageRequestResponse);
            Assert.True(result.Status);
            Assert.True(File.Exists(@"C:\MySynch.Dest.Test.Root\File1.xml"));

        }

        [Test]
        [Ignore(@"Requires the subscriber to be started and responsive on the media-centre machine")]
        public void SubscriberRemoteGetHeartBeat_OK()
        {
            ISubscriberProxy subscriberProxy = new SubscriberClient();
            subscriberProxy.InitiateUsingPort(8767);
            var heartBeatResponse = subscriberProxy.GetHeartbeat();
            Assert.IsNotNull(heartBeatResponse);
            Assert.True(heartBeatResponse.Status);
        }



    }
}
