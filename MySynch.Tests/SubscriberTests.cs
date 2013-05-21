using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Moq;
using MySynch.Contracts.Messages;
using MySynch.Core;
using MySynch.Core.DataTypes;
using MySynch.Core.Interfaces;
using MySynch.Core.Subscriber;
using MySynch.Tests.Stubs;
using NUnit.Framework;

namespace MySynch.Tests
{
    [TestFixture]
    public class SubscriberTests
    {
        private bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open,
                         FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        [SetUp]
        public void SetUp()
        {
            if (IsFileLocked(new FileInfo(@"Data\Output\Test\F1\F12\F12.xml")))
                Thread.Sleep(500);
            File.Copy(@"..\..\Data\Output\Test\F1\F12\F12.xml", @"Data\Output\Test\F1\F12\F12.xml", true);
        }

        [Test]
        public void ApplyChanges_Upserts_Ok()
        {
            MySynchComponentResolver mySynchComponentResolver= new MySynchComponentResolver();
            mySynchComponentResolver.RegisterAll(new TestInstaller());

            Subscriber subscriber = new Subscriber(mySynchComponentResolver);

            LocalComponentConfig localComponentConfig = new LocalComponentConfig
                                                            {BrokerName = "", RootFolder = @"Data\Output\Test\"};
            string hostUrl=string.Empty;
            subscriber.Initialize(null,localComponentConfig,hostUrl);
            var mockCopyStrategy = MockCopyStrategy();
            subscriber.InitiatedCopyStrategies = new SortedList<string, CopyStrategy>
                                                     {{"publisher url", mockCopyStrategy}};

            ReceiveMessageRequest request = new ReceiveMessageRequest
            {
                PublisherMessage = new PublisherMessage()
                {
                    AbsolutePath = @"Data\Test\F1\F12\F12.xml",
                    OperationType = OperationType.Insert,
                    SourceOfMessageUrl="publisher url",
                    SourcePathRootName=@"Data\Test\"
                }
            };
            var response = subscriber.ReceiveMessage(request);
            Assert.IsTrue(response.Success);
        }

        private CopyStrategy MockCopyStrategy()
        {
            var mockCopyStrategy = new Mock<CopyStrategy>();
            mockCopyStrategy.Setup(m => m.Copy(@"Data\Test\F1\F12\F12.xml", @"Data\Output\Test\F1\F12\F12.xml")).Returns(true);
            mockCopyStrategy.Setup(m => m.Copy(@"Data\Test\F1\F13\F13.xml", @"Data\Output\Test\F1\F13\F13.xml")).Returns(true);
            mockCopyStrategy.Setup(m => m.Copy(@"Data\Test\F1\F13\F12.xml", @"Data\Output\Test\F1\F13\F12.xml")).Returns(false);
            mockCopyStrategy.Setup(m => m.Copy(@"Data\Test\F1\F12\F11.xml", @"Data\Output\Test\F1\F12\F11.xml")).Returns(true);
            mockCopyStrategy.Setup(m => m.Copy(@"root folder\Item One", @"destination root folder\Item One")).Returns(true);
            mockCopyStrategy.Setup(m => m.Copy(@"root folder\ItemTwo", @"destination root folder\ItemTwo")).Returns(true);
            return mockCopyStrategy.Object;
        }

        [Test]
        public void ApplyChanges_Deletes_Ok()
        {
            MySynchComponentResolver mySynchComponentResolver = new MySynchComponentResolver();
            mySynchComponentResolver.RegisterAll(new TestInstaller());

            Subscriber subscriber = new Subscriber(mySynchComponentResolver);

            LocalComponentConfig localComponentConfig = new LocalComponentConfig { BrokerName = "", RootFolder = @"Data\Output\Test\" };
            string hostUrl = string.Empty;
            subscriber.Initialize(null, localComponentConfig, hostUrl);

            Assert.True(File.Exists(@"Data\Output\Test\F1\F12\F12.xml"));
            var mockCopyStrategy = MockCopyStrategy();
            subscriber.InitiatedCopyStrategies = new SortedList<string, CopyStrategy> { { "publisher url", mockCopyStrategy } };
            ReceiveMessageRequest request = new ReceiveMessageRequest
            {
                PublisherMessage = new PublisherMessage()
                {
                    AbsolutePath = @"Data\Test\F1\F12\F12.xml",
                    OperationType = OperationType.Delete,
                    SourceOfMessageUrl = "publisher url",
                    SourcePathRootName = @"Data\Test\"
                }
            };
            var response = subscriber.ReceiveMessage(request);
            Assert.True(response.Success);
            Assert.False(File.Exists(@"Data\Output\Test\F1\F12\F12.xml"));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Initialize_NonExistentFolder()
        {
            MySynchComponentResolver componentResolver= new MySynchComponentResolver();
            Subscriber subscriber= new Subscriber(componentResolver);
            LocalComponentConfig localComponentConfig= new LocalComponentConfig
                                                           {
                                                               BrokerName="",
                                                               RootFolder = @"c:\mewrongdata\mewrongfoldero"
                                                           };
            subscriber.Initialize(null,localComponentConfig, string.Empty);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReceiveMessage_NoMessgaeSent()
        {
            MySynchComponentResolver mySynchComponentResolver = new MySynchComponentResolver();
            mySynchComponentResolver.RegisterAll(new TestInstaller());

            Subscriber subscriber = new Subscriber(mySynchComponentResolver);

            LocalComponentConfig localComponentConfig = new LocalComponentConfig { BrokerName = "", RootFolder = @"Data\Output\Test\" };
            string hostUrl = string.Empty;
            subscriber.Initialize(null, localComponentConfig, hostUrl);

            subscriber.ReceiveMessage(null);
        }

        [Test]
        public void ReceiveMessage_NoAbsolutePath()
        {
            MySynchComponentResolver mySynchComponentResolver = new MySynchComponentResolver();
            mySynchComponentResolver.RegisterAll(new TestInstaller());

            Subscriber subscriber = new Subscriber(mySynchComponentResolver);

            LocalComponentConfig localComponentConfig = new LocalComponentConfig { BrokerName = "", RootFolder = @"Data\Output\Test\" };
            string hostUrl = string.Empty;
            subscriber.Initialize(null, localComponentConfig, hostUrl);

            ReceiveMessageRequest request = new ReceiveMessageRequest
            {
                PublisherMessage = new PublisherMessage()
                {
                    AbsolutePath = "",
                    OperationType = OperationType.Insert,
                    SourceOfMessageUrl = "publisher url",
                    SourcePathRootName = @"Data\Test\"
                }
            };
            var response = subscriber.ReceiveMessage(request);
            Assert.IsFalse(response.Success);
        }

        [Test]
        public void GetOrCreateCopyStrategy_CreateOk()
        {
            MySynchComponentResolver mySynchComponentResolver = new MySynchComponentResolver();
            mySynchComponentResolver.RegisterAll(new TestInstaller());

            Subscriber subscriber = new Subscriber(mySynchComponentResolver);

            LocalComponentConfig localComponentConfig = new LocalComponentConfig { BrokerName = "", RootFolder = @"Data\Output\Test\" };
            string hostUrl = string.Empty;
            subscriber.Initialize(null, localComponentConfig, hostUrl);
            subscriber.InitiatedCopyStrategies = new SortedList<string, CopyStrategy> ();

            var copyStrategy = subscriber.GetOrCreateCopyStrategy("new publisher url");

            Assert.IsNotNull(copyStrategy);
            Assert.IsNotNull(subscriber.InitiatedCopyStrategies["new publisher url"]);
            Assert.AreEqual(1,subscriber.InitiatedCopyStrategies.Count);
        }

        [Test]
        public void GetOrCreateCopyStrategy_GetOk()
        {
            MySynchComponentResolver mySynchComponentResolver = new MySynchComponentResolver();
            mySynchComponentResolver.RegisterAll(new TestInstaller());

            Subscriber subscriber = new Subscriber(mySynchComponentResolver);

            LocalComponentConfig localComponentConfig = new LocalComponentConfig { BrokerName = "", RootFolder = @"Data\Output\Test\" };
            string hostUrl = string.Empty;
            subscriber.Initialize(null, localComponentConfig, hostUrl);
            var mockCopyStrategy = MockCopyStrategy();
            subscriber.InitiatedCopyStrategies = new SortedList<string, CopyStrategy> { { "old publisher url", mockCopyStrategy } };

            var copyStrategy = subscriber.GetOrCreateCopyStrategy("old publisher url");

            Assert.IsNotNull(copyStrategy);
            Assert.False(subscriber.InitiatedCopyStrategies.ContainsKey("new publisher url"));
            Assert.AreEqual(1, subscriber.InitiatedCopyStrategies.Count);
            Assert.IsNotNull(subscriber.InitiatedCopyStrategies["old publisher url"]);
        }

        [Test]
        public void GetHeartbeat_Ok()
        {
            MySynchComponentResolver componentResolver = new MySynchComponentResolver();
            Subscriber subscriber = new Subscriber(componentResolver);
            LocalComponentConfig localComponentConfig = new LocalComponentConfig
            {
                BrokerName = "",
                RootFolder = @"."
            };
            subscriber.Initialize(null, localComponentConfig, string.Empty);
            var heartbeatResponse = subscriber.GetHeartbeat();
            Assert.True(heartbeatResponse.Status);
            Assert.AreEqual(".",heartbeatResponse.RootPath);

        }
    }
}
