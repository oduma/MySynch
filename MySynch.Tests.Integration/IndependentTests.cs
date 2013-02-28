﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MySynch.Contracts.Messages;
using MySynch.Core.Subscriber;
using MySynch.Proxies;
using MySynch.Proxies.Interfaces;
using NUnit.Framework;

namespace MySynch.Tests.Integration
{
    [TestFixture]
    [Category("integration")]
    [Ignore("Start the servives at the specified address and after that this should run")]
    public class IndependentTests
    {
        [Test]
        public void PublisherServiceUpAndAccessible()
        {
            IPublisherProxy publisherProxy = new PublisherClient();
            publisherProxy.InitiateUsingEndpoint("PublisherSciendoLaptop");
            var publishedPackage = publisherProxy.PublishPackage();
            Assert.IsNull(publishedPackage);
        }

        [Test]
        [Ignore(@"Assumes that the publisher is watching the folder C:\MySynch.Source.Test.Root\")]
        public void PublisherServicePublishAPackage()
        {
            File.Copy(@"Data\File1.xml", @"C:\MySynch.Source.Test.Root\File1.xml",true);
            IPublisherProxy publisherProxy = new PublisherClient();
            publisherProxy.InitiateUsingEndpoint("PublisherSciendoLaptop");
            var publishedPackage = publisherProxy.PublishPackage();
            Assert.IsNotNull(publishedPackage);
            Assert.AreEqual("SCIENDO-LAPTOP",publishedPackage.Source);
            Assert.AreEqual(@"C:\MySynch.Source.Test.Root\", publishedPackage.SourceRootName);
            Assert.IsNotNull(publishedPackage.PackageId);
            Assert.IsNotNull( publishedPackage.ChangePushItems);
            Assert.AreEqual(1, publishedPackage.ChangePushItems.Count);
            Assert.AreEqual(@"C:\MySynch.Source.Test.Root\File1.xml", publishedPackage.ChangePushItems[0].AbsolutePath);
            Assert.AreEqual(OperationType.Update, publishedPackage.ChangePushItems[0].OperationType);
            publisherProxy.RemovePackage(publishedPackage);
        }

        [Test]
        [Ignore(@"Requires the file C:\MySynch.Source.Test.Root\File1.xml to be present")]
        public void DataSourceUpAndAccessible()
        {

            ISourceOfDataProxy sourceOfDataProxy = new SourceOfDataClient();
            sourceOfDataProxy.InitiateUsingEndpoint("SourceOfDataSciendoLaptop");
            var data = sourceOfDataProxy.GetData(new RemoteRequest {FileName = @"C:\MySynch.Source.Test.Root\File1.xml"});
            Assert.IsNotNull(data);
            Assert.IsNotNull(data.Data);
            UTF8Encoding encoding = new UTF8Encoding();
            var content = encoding.GetString(data.Data);
            Assert.AreEqual(@"﻿<?xml version=""1.0"" encoding=""utf-8"" ?>
<ab>
  <ba>abbba</ba>
</ab>
",content);
        }

        [Test]
        public void SubscriberUpAndAccessible()
        {
            ISubscriberProxy subscriberProxy = new SubscriberClient();
            subscriberProxy.InitiateUsingEndpoint("SubscriberSciendoLaptop");
            Assert.True(subscriberProxy.TryOpenChannel(new TryOpenChannelRequest{SourceOfDataEndpointName="SourceOfDataSciendoLaptop"}).Status);
            var result = subscriberProxy.ApplyChangePackage(new PublishPackageRequestResponse());
            Assert.False(result.Status);
            
        }

        [Test]
        [Ignore(@"Assumes presence of C:\MySynch.Source.Test.Root\File1.xml")]
        public void TestOnlyRemoteDataSource_Ok()
        {
            if (File.Exists(@"Data\Output\File1.xml"))
                File.Delete(@"Data\Output\File1.xml");
            Subscriber subscriber = new Subscriber();
            subscriber.Initialize(@"Data\Output");
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
            Assert.True(subscriber.TryOpenChannel(new TryOpenChannelRequest{SourceOfDataEndpointName="SourceOfDataSciendoLaptop"}).Status);
            var result = subscriber.ApplyChangePackage(publishedPackageRequestResponse);
            Assert.True(result.Status);
            Assert.True(File.Exists(@"Data\Output\File1.xml"));
        }

        [Test]
        [Ignore(@"Requires Subscriber service to be defined on the root folder: C:\MySynch.Dest.Test.Root\")]
        public void SubscriberApplyChanges_Ok()
        {
            if(File.Exists(@"C:\MySynch.Dest.Test.Root\File1.xml"))
                File.Delete(@"C:\MySynch.Dest.Test.Root\File1.xml");
            ISubscriberProxy subscriberProxy = new SubscriberClient();
            subscriberProxy.InitiateUsingEndpoint("SubscriberSciendoLaptop");
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
            Assert.True(subscriberProxy.TryOpenChannel(new TryOpenChannelRequest{SourceOfDataEndpointName="SourceOfDataSciendoLaptop"}).Status);
            var result = subscriberProxy.ApplyChangePackage(publishedPackageRequestResponse);
            Assert.True(result.Status);
            Assert.True(File.Exists(@"C:\MySynch.Dest.Test.Root\File1.xml"));

        }

    }
}
