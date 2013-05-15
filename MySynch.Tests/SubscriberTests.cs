using System.Collections.Generic;
using System.IO;
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
        [SetUp]
        public void SetUp()
        {
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
            subscriber.InitiatedCopyStrategies = new SortedList<string, ICopyStrategy>
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

        private ICopyStrategy MockCopyStrategy()
        {
            var mockCopyStrategy = new Mock<ICopyStrategy>();
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
            subscriber.InitiatedCopyStrategies = new SortedList<string, ICopyStrategy> { { "publisher url", mockCopyStrategy } };
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

    }
}
