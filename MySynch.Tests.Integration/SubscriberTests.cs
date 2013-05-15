using NUnit.Framework;

namespace MySynch.Tests.Integration
{
    [TestFixture]
    [Category("integration")]
    [Ignore("Start the servives at the specified address and after that this should run")]
    public class SubscriberTests
    {
    //    [Test]
    //    public void SubscriberUpAndAccessible()
    //    {
    //        ISubscriberProxy subscriberProxy = new SubscriberClient();
    //        subscriberProxy.InitiateUsingPort(8765);
    //        Assert.True(subscriberProxy.TryOpenChannel(new TryOpenChannelRequest { SourceOfDataPort = 8765 }).Status);
    //        Assert.False(
    //            subscriberProxy.ReceiveMessage(new ReceiveMessageRequest
    //                                                    {
    //                                                        PublisherMessage = new ChangePushItem(),
    //                                                        SourceRootName = string.Empty
    //                                                    }).Success);
    //    }

        //[Test]
        //[Ignore(@"Assumes presence of C:\MySynch.Source.Test.Root\File1.xml")]
        //public void TestOnlyRemoteDataSource_Ok()
        //{
        //    if (File.Exists(@"Data\Output\File1.xml"))
        //        File.Delete(@"Data\Output\File1.xml");
        //    MySynchComponentResolver mySynchComponentResolver = new MySynchComponentResolver();
        //    mySynchComponentResolver.RegisterAll(new MySynchInstaller());
        //    Subscriber subscriber = new Subscriber(mySynchComponentResolver);
        //    subscriber.Initialize(null,
        //                          new LocalComponentConfig { BrokerName = "Broker name", RootFolder = @"Data\Output" }, "");
        //    PublisherMessage publisherMessage = new PublisherMessage
        //    {
        //        SourceOfMessageUrl = @"http://localhost/publisher",
        //        SourcePathRootName = @"C:\MySynch.Source.Test.Root\",
        //        AbsolutePath = @"C:\MySynch.Source.Test.Root\File1.xml",
        //        OperationType = OperationType.Insert
        //    };

        //    Assert.True(subscriber.ReceiveMessage(new ReceiveMessageRequest { PublisherMessage = publisherMessage }).Success);
        //    Assert.True(File.Exists(@"Data\Output\File1.xml"));
        //}


    //    [Test]
    //    [Ignore(@"Requires Subscriber service to be defined on the root folder: C:\MySynch.Dest.Test.Root\")]
    //    public void SubscriberApplyChanges_Ok()
    //    {
    //        if (File.Exists(@"C:\MySynch.Dest.Test.Root\File1.xml"))
    //            File.Delete(@"C:\MySynch.Dest.Test.Root\File1.xml");
    //        ISubscriberProxy subscriberProxy = new SubscriberClient();
    //        subscriberProxy.InitiateUsingPort(8765);
    //        ReceiveMessageRequest receiveMessageRequest = new ReceiveMessageRequest
    //        {
    //            SourceRootName = @"C:\MySynch.Source.Test.Root\",
    //            PublisherMessage =
    //                                                                 new ChangePushItem
    //                                                                     {
    //                                                                         AbsolutePath =
    //                                                                             @"C:\MySynch.Source.Test.Root\File1.xml",
    //                                                                         OperationType = OperationType.Insert
    //                                                                     }
    //        };
    //        Assert.True(subscriberProxy.TryOpenChannel(new TryOpenChannelRequest { SourceOfDataPort = 8765 }).Status);
    //        subscriberProxy.ReceiveMessage(receiveMessageRequest);
    //        Assert.True(File.Exists(@"C:\MySynch.Dest.Test.Root\File1.xml"));

    //    }

    //    [Test]
    //    [Ignore(@"Requires the subscriber to be started and responsive on the media-centre machine")]
    //    public void SubscriberRemoteGetHeartBeat_OK()
    //    {
    //        ISubscriberProxy subscriberProxy = new SubscriberClient();
    //        subscriberProxy.InitiateUsingPort(8767);
    //        var heartBeatResponse = subscriberProxy.GetHeartbeat();
    //        Assert.IsNotNull(heartBeatResponse);
    //        Assert.True(heartBeatResponse.Status);
    //    }



    }
}
