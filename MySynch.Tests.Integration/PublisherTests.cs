using System;
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
    public class PublisherTests
    {
        [Test]
        public void PublisherServiceUpAndAccessible()
        {
            IPublisherProxy publisherProxy = new PublisherClient();
            publisherProxy.InitiateUsingPort(8765);
            var publishedPackage = publisherProxy.PublishPackage();
            Assert.IsNull(publishedPackage);
        }

        [Test]
        [Ignore(@"Assumes that the publisher is watching the folder C:\MySynch.Source.Test.Root\")]
        public void PublisherServicePublishAPackage()
        {
            File.Copy(@"Data\File1.xml", @"C:\MySynch.Source.Test.Root\File1.xml",true);
            IPublisherProxy publisherProxy = new PublisherClient();
            publisherProxy.InitiateUsingPort(8765);
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
            sourceOfDataProxy.InitiateUsingPort(8765);
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
            Assert.True(subscriber.TryOpenChannel(new TryOpenChannelRequest{SourceOfDataPort=8765}).Status);
            var result = subscriber.ApplyChangePackage(publishedPackageRequestResponse);
            Assert.True(result.Status);
            Assert.True(File.Exists(@"Data\Output\File1.xml"));
        }

        [Test]
        public void PublisherStartsWithoutABackupFile()
        {
            Assert.Fail();
        }

        [Test]
        public void PublisherStartsWithABackupFile()
        {
            Assert.Fail();
        }

        [Test]
        public void PublisherSavingABackupFileOnStop_NothingToSave()
        {
            Assert.Fail();
        }

        [Test]
        public void PublisherSavingABackupFileOnStop()
        {
            Assert.Fail();
        }
    }
}
