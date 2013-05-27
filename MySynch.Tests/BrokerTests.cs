﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using MySynch.Common.IOC;
using MySynch.Common.Serialization;
using MySynch.Contracts.Messages;
using MySynch.Core;
using MySynch.Core.Broker;
using MySynch.Core.DataTypes;
using MySynch.Proxies.Autogenerated.Interfaces;
using MySynch.Tests.Stubs;
using NUnit.Framework;

namespace MySynch.Tests
{
    [TestFixture]
    public class BrokerTests
    {
        [SetUp]
        public void SetUp()
        {
            File.Copy(@"..\..\Data\storetestget.xml",@"Data\storetestget.xml",true);
            File.Copy(@"..\..\Data\storetest.xml", @"Data\storetest.xml", true);

        }
        #region Attach an Related
        [Test]
        public void AddRegistration_Ok()
        {
            List<Registration> registrations = new List<Registration> { new Registration { ServiceUrl = "my reg 1" } };
            registrations.AddRegistration(new Registration { ServiceUrl = "my reg 2" });
            Assert.AreEqual(2, registrations.Count);
        }

        [Test]
        public void AddRegistration_OnNewList()
        {
            List<Registration> registrations = new List<Registration>();
            registrations.AddRegistration(new Registration { ServiceUrl = "my reg 2" });
            Assert.AreEqual(1, registrations.Count);
        }

        [Test]
        public void AddRegistration_OnNull()
        {
            List<Registration> registrations = null;
            var resultRegistrations = registrations.AddRegistration(new Registration { ServiceUrl = "my reg 2" });
            Assert.AreEqual(1, resultRegistrations.Count);
        }

        [Test]
        public void SaveAndReturn_WithBackEndXml_Ok()
        {
            var store = new FileSystemStore<Registration>();
            List<Registration> registrations = new List<Registration> { new Registration { ServiceUrl = "my reg 1", ServiceRole = ServiceRole.Publisher } };
            registrations.SaveAndReturn("storeName1.xml", store.StoreMethod);
            Assert.AreEqual(1, registrations.Count);
            store = new FileSystemStore<Registration>();
            var fromStore = store.GetMethod("storeName1.xml");
            Assert.AreEqual(1, fromStore.Count);
            Assert.True(fromStore.Any(a => a.ServiceUrl == "my reg 1"));
        }

        [Test]
        public void SaveAnReturn_WithBackEndXml_OnNewList()
        {
            var store = new FileSystemStore<Registration>();
            List<Registration> registrations = new List<Registration>();
            registrations.SaveAndReturn("storeName1.xml", store.StoreMethod);
            store = new FileSystemStore<Registration>();
            var fromStore = store.GetMethod("storeName1.xml");
            Assert.IsNotNull(fromStore);
            Assert.AreEqual(0, fromStore.Count);
        }

        [Test]
        public void SaveAndReturn_WithBackEndXml_OnNull()
        {
            var store = new FileSystemStore<Registration>();
            List<Registration> registrations = null;
            registrations.SaveAndReturn("storeName1.xml", store.StoreMethod);
            store = new FileSystemStore<Registration>();
            var fromStore = store.GetMethod("storeName1.xml");
            Assert.IsNotNull(fromStore);
            Assert.AreEqual(0, fromStore.Count);
        }

        [Test]
        public void SaveAndReturn_WithBackEndMemory_Ok()
        {
            var store = new MemoryStore<Registration>();
            List<Registration> registrations = new List<Registration> { new Registration { ServiceUrl = "my reg 1", ServiceRole = ServiceRole.Publisher } };
            registrations.SaveAndReturn("storeName1", store.StoreMethod);
            Assert.AreEqual(1, registrations.Count);
            var fromStore = store.GetMethod("storeName1");
            Assert.AreEqual(1, fromStore.Count);
            Assert.True(fromStore.Any(a => a.ServiceUrl == "my reg 1"));
        }

        [Test]
        public void SaveAnReturn_WithBackEndMemory_OnNewList()
        {
            var store = new MemoryStore<Registration>();
            List<Registration> registrations = new List<Registration>();
            registrations.SaveAndReturn("storeName1", store.StoreMethod);
            var fromStore = store.GetMethod("storeName1");
            Assert.IsNotNull(fromStore);
            Assert.AreEqual(0, fromStore.Count);
        }

        [Test]
        public void SaveAndReturn_WithBackEndMemory_OnNull()
        {
            var store = new MemoryStore<Registration>();
            List<Registration> registrations = null;
            registrations.SaveAndReturn("storeName1", store.StoreMethod);
            var fromStore = store.GetMethod("storeName1");
            Assert.IsNotNull(fromStore);
            Assert.AreEqual(0, fromStore.Count);
        }

        [Test]
        public void Attach_No_ValidRegistration()
        {
            StoreType storeType = new StoreType { StoreName = "store1.xml", StoreTypeName = "IStore.Registration.FileSystemStore" };
            ComponentResolver componentResolver = new ComponentResolver();
            componentResolver.RegisterAll(new AllStoresInstaller());
            Broker broker = new Broker(storeType, componentResolver);
            AttachRequest request = new AttachRequest();
            var resonse = broker.Attach(request);
            Assert.False(resonse.RegisteredOk);
        }

        [Test]
        public void Attach_Ok()
        {
            StoreType storeType = new StoreType { StoreName = "store1.xml", StoreTypeName = "IStore.Registration.FileSystemStore" };
            ComponentResolver componentResolver = new ComponentResolver();
            componentResolver.RegisterAll(new AllStoresInstaller());
            Broker broker = new Broker(storeType, componentResolver);
            AttachRequest request = new AttachRequest
            {
                RegistrationRequest =
                    new Registration
                    {
                        OperationTypes =
                            new List<OperationType>
                                                                {
                                                                    OperationType.Insert,
                                                                    OperationType.Update,
                                                                    OperationType.Delete
                                                                },
                        ServiceRole = ServiceRole.Publisher,
                        ServiceUrl = "Service Url"
                    }
            };
            var resonse = broker.Attach(request);
            Assert.True(resonse.RegisteredOk);
        }

        [Test]
        public void Attach_AlreadyAttached()
        {
            StoreType storeType = new StoreType { StoreName = "store1.xml", StoreTypeName = "IStore.Registration.FileSystemStore" };
            ComponentResolver componentResolver = new ComponentResolver();
            componentResolver.RegisterAll(new AllStoresInstaller());
            Broker broker = new Broker(storeType, componentResolver);
            AttachRequest request = new AttachRequest
            {
                RegistrationRequest =
                    new Registration
                    {
                        OperationTypes =
                            new List<OperationType>
                                                                {
                                                                    OperationType.Insert,
                                                                    OperationType.Update,
                                                                    OperationType.Delete
                                                                },
                        ServiceRole = ServiceRole.Publisher,
                        ServiceUrl = "Service Url"
                    }
            };
            var resonse = broker.Attach(request);
            Assert.True(resonse.RegisteredOk);
        }

        [Test]
        public void Attach_Error()
        {
            StoreType storeType = new StoreType { StoreName = "store1.xml", StoreTypeName = "IStore.Registration.DeffectiveStore" };
            ComponentResolver componentResolver = new ComponentResolver();
            componentResolver.RegisterAll(new AllStoresInstaller());
            Broker broker = new Broker(storeType, componentResolver);
            AttachRequest request = new AttachRequest
            {
                RegistrationRequest =
                    new Registration
                    {
                        OperationTypes =
                            new List<OperationType>
                                                                {
                                                                    OperationType.Insert,
                                                                    OperationType.Update,
                                                                    OperationType.Delete
                                                                },
                        ServiceRole = ServiceRole.Publisher,
                        ServiceUrl = "Service Url"
                    }
            };
            var resonse = broker.Attach(request);
            Assert.False(resonse.RegisteredOk);
        }
        #endregion

        #region Detach and Related
        [Test]
        public void RemoveRegistration_NotFound()
        {
            List<Registration> registrations = new List<Registration> { new Registration { ServiceUrl = "my reg 1" } };
            Registration deletedCopy;
            registrations.RemoveRegistration("my reg 2",out deletedCopy);
            Assert.AreEqual(1, registrations.Count);
            Assert.IsNull(deletedCopy);
        }

        [Test]
        public void RemoveRegistration_Ok()
        {
            List<Registration> registrations = new List<Registration> { new Registration { ServiceUrl = "my reg 1" }, new Registration { ServiceUrl = "my reg 2" } };
            Registration deletedCopy;
            registrations.RemoveRegistration("my reg 2", out deletedCopy);
            Assert.AreEqual(1, registrations.Count);
            Assert.IsNotNull(deletedCopy);
            Assert.AreEqual("my reg 2",deletedCopy.ServiceUrl);
        }
        [Test]
        public void RemoveRegistration_OnNewList()
        {
            List<Registration> registrations = new List<Registration>();
            Registration deletedCopy;
            registrations.RemoveRegistration("my reg 2", out deletedCopy);
            Assert.IsNotNull(registrations);
            Assert.AreEqual(0, registrations.Count);
            Assert.IsNull(deletedCopy);
        }

        [Test]
        public void RemoveRegistration_OnNull()
        {
            List<Registration> registrations = null;
            Registration deletedCopy;
            var resultRegistrations = registrations.RemoveRegistration("my reg 2",out deletedCopy);
            Assert.IsNotNull(resultRegistrations);
            Assert.AreEqual(0, resultRegistrations.Count);
            Assert.IsNull(deletedCopy);
        }

        [Test]
        public void Detach_No_ValidRegistration()
        {
            StoreType storeType = new StoreType { StoreName = "store1.xml", StoreTypeName = "IStore.Registration.FileSystemStore" };
            ComponentResolver componentResolver = new ComponentResolver();
            componentResolver.RegisterAll(new AllStoresInstaller());
            Broker broker = new Broker(storeType, componentResolver);
            DetachRequest request = new DetachRequest();
            var resonse = broker.Detach(request);
            Assert.False(resonse.Status);
        }

        [Test]
        public void Detach_Ok()
        {
            StoreType storeType = new StoreType { StoreName = @"Data\storetest.xml", StoreTypeName = "IStore.Registration.FileSystemStore" };
            ComponentResolver componentResolver = new ComponentResolver();
            componentResolver.RegisterAll(new AllStoresInstaller());
            Broker broker = new Broker(storeType, componentResolver);

            DetachRequest detachRequest = new DetachRequest { ServiceUrl = "Service Url1" };

            var response = broker.Detach(detachRequest);
            Assert.True(response.Status);
        }

        [Test]
        public void Detach_NotFound()
        {
            StoreType storeType = new StoreType { StoreName = @"Data\storetest.xml", StoreTypeName = "IStore.Registration.FileSystemStore" };
            ComponentResolver componentResolver = new ComponentResolver();
            componentResolver.RegisterAll(new AllStoresInstaller());
            Broker broker = new Broker(storeType, componentResolver);
            DetachRequest request = new DetachRequest { ServiceUrl = "Non existent url" };
            var resonse = broker.Detach(request);
            Assert.False(resonse.Status);
        }

        [Test]
        public void Detach_Error()
        {
            StoreType storeType = new StoreType { StoreName = @"Data\storetest.xml", StoreTypeName = "IStore.Registration.DeffectiveStore" };
            ComponentResolver componentResolver = new ComponentResolver();
            componentResolver.RegisterAll(new AllStoresInstaller());
            Broker broker = new Broker(storeType, componentResolver);
            DetachRequest request = new DetachRequest { ServiceUrl = "Service Url2" };
            var resonse = broker.Detach(request);
            Assert.False(resonse.Status);
        }
        #endregion

        #region Attach and Detach 
        [Test]
        public void ListAllRegistrations_Ok()
        {
            StoreType storeType = new StoreType { StoreName = @"Data\storetestget.xml", StoreTypeName = "IStore.Registration.FileSystemStore" };
            ComponentResolver componentResolver = new ComponentResolver();
            componentResolver.RegisterAll(new AllStoresInstaller());
            Broker broker = new Broker(storeType, componentResolver);
            var response = broker.ListAllRegistrations();
            Assert.IsNotNull(response.Registrations);
            Assert.AreEqual(3, response.Registrations.Count);
            Assert.False(response.Registrations.Any(r => r.ServiceRole == ServiceRole.Subscriber));
        }
        [Test]
        public void ListAllRegistrations_After_AnInsert()
        {
            StoreType storeType = new StoreType { StoreName = @"Data\storetestget.xml", StoreTypeName = "IStore.Registration.FileSystemStore" };
            ComponentResolver componentResolver = new ComponentResolver();
            componentResolver.RegisterAll(new AllStoresInstaller());
            Broker broker = new Broker(storeType, componentResolver);
            AttachRequest request = new AttachRequest
            {
                RegistrationRequest =
                    new Registration
                    {
                        OperationTypes =
                            new List<OperationType>
                                                                {
                                                                    OperationType.Insert,
                                                                    OperationType.Update,
                                                                    OperationType.Delete
                                                                },
                        ServiceRole = ServiceRole.Subscriber,
                        ServiceUrl = "Service Url3"
                    }
            };
            broker.Attach(request);

            var response = broker.ListAllRegistrations();
            Assert.IsNotNull(response.Registrations);
            Assert.AreEqual(4, response.Registrations.Count);
            Assert.True(response.Registrations.Any(r => r.ServiceRole == ServiceRole.Subscriber));
        }
        [Test]
        public void ListAllRegistrations_AfterADelete()
        {
            StoreType storeType = new StoreType { StoreName = @"Data\storetestget.xml", StoreTypeName = "IStore.Registration.FileSystemStore" };
            ComponentResolver componentResolver = new ComponentResolver();
            componentResolver.RegisterAll(new AllStoresInstaller());
            Broker broker = new Broker(storeType, componentResolver);
            DetachRequest detachRequest = new DetachRequest { ServiceUrl = "Service Url1" };
            broker.Detach(detachRequest);

            var response = broker.ListAllRegistrations();
            Assert.IsNotNull(response.Registrations);
            Assert.AreEqual(2, response.Registrations.Count(r => r.ServiceRole == ServiceRole.Publisher));
        }
        [Test]
        public void ListAllRegistrations_Error()
        {
            StoreType storeType = new StoreType { StoreName = @"Data\storetestget.xml", StoreTypeName = "IStore.Registration.DeffectiveGetStore" };
            ComponentResolver componentResolver = new ComponentResolver();
            componentResolver.RegisterAll(new AllStoresInstaller());
            Broker broker = new Broker(storeType, componentResolver);
            var response = broker.ListAllRegistrations();
            Assert.IsNotNull(response.Registrations);
            Assert.AreEqual(0, response.Registrations.Count);
        }
        [Test]
        public void ListAllRegistrations_WrongFile()
        {
            StoreType storeType = new StoreType { StoreName = @"Data\wrongfile.xml", StoreTypeName = "IStore.Registration.FileSystemStore" };
            ComponentResolver componentResolver = new ComponentResolver();
            componentResolver.RegisterAll(new AllStoresInstaller());
            Broker broker = new Broker(storeType, componentResolver);
            var response = broker.ListAllRegistrations();
            Assert.IsNotNull(response.Registrations);
            Assert.AreEqual(0, response.Registrations.Count);
        }

        #endregion

        #region Distribute Messages Related
        [Test]
        public void ReceiveAndDistributeMessage_Ok()
        {
            StoreType storeType = new StoreType { StoreName = "store1.xml", StoreTypeName = "IStore.Registration.FileSystemStore" };
            ComponentResolver componentResolver = new ComponentResolver();
            componentResolver.RegisterAll(new AllStoresInstaller());
            Broker broker = new Broker(storeType, componentResolver);
            ReceiveAndDistributeMessageRequest mRequest = new ReceiveAndDistributeMessageRequest
                                                              {
                                                                  PublisherMessage = 
                                                                      new PublisherMessage
                                                                          {
                                                                              AbsolutePath = "abc",
                                                                              OperationType = OperationType.Insert
                                                                          }
                                                              };
            broker.ReceiveAndDistributeMessage(mRequest);
            var mResponse = broker.ListAllMessages();
            Assert.IsNotNull(mResponse);
            Assert.IsNotNull(mResponse.AvailableMessages);
            Assert.AreEqual(1,mResponse.AvailableMessages.Count);
        }

        [Test]
        public void ListAllMessages_Ok()
        {
            StoreType storeType = new StoreType { StoreName = "store1.xml", StoreTypeName = "IStore.Registration.FileSystemStore" };
            ComponentResolver componentResolver = new ComponentResolver();
            componentResolver.RegisterAll(new AllStoresInstaller());
            Broker broker = new Broker(storeType, componentResolver);
            ReceiveAndDistributeMessageRequest mRequest = new ReceiveAndDistributeMessageRequest
            {
                PublisherMessage =
                    new PublisherMessage
                    {
                        AbsolutePath = "abc",
                        OperationType = OperationType.Insert
                    }
            };
            broker.ReceiveAndDistributeMessage(mRequest);
            var mResponse = broker.ListAllMessages();
            Assert.IsNotNull(mResponse);
            Assert.IsNotNull(mResponse.AvailableMessages);
            Assert.AreEqual(1, mResponse.AvailableMessages.Count);
        }

        [Test]
        public void GetOrCreateAProxy_NothingRegistered()
        {
            StoreType storeType = new StoreType { StoreName = "store1.xml", StoreTypeName = "IStore.Registration.FileSystemStore" };
            ComponentResolver componentResolver = new ComponentResolver();
            componentResolver.RegisterAll(new AllStoresInstaller());
            Broker broker = new Broker(storeType, componentResolver);
            Assert.IsNull(broker.GetOrCreateAProxy("new subscriber url"));
        }

        [Test]
        public void GetOrCreateAProxy_Create_Ok()
        {
            StoreType storeType = new StoreType { StoreName = "store1.xml", StoreTypeName = "IStore.Registration.FileSystemStore" };
            ComponentResolver componentResolver = new ComponentResolver();
            componentResolver.RegisterAll(new TestInstaller());
            Broker broker = new Broker(storeType, componentResolver);
            broker.InitiatedSubScriberProxies = new SortedList<string, ISubscriberProxy> {{"old subscriber", null}};
            var response = broker.GetOrCreateAProxy("new subscriber proxy");
            Assert.IsNotNull(response);
            Assert.AreEqual("It'smy party", response.GetHeartbeat().RootPath);
            Assert.AreEqual(2,broker.InitiatedSubScriberProxies.Count);
        }

        [Test]
        public void GetOrCreateAProxy_Get_Ok()
        {
            StoreType storeType = new StoreType { StoreName = "store1.xml", StoreTypeName = "IStore.Registration.FileSystemStore" };
            ComponentResolver componentResolver = new ComponentResolver();
            componentResolver.RegisterAll(new TestInstaller());
            Broker broker = new Broker(storeType, componentResolver);
            broker.InitiatedSubScriberProxies = new SortedList<string, ISubscriberProxy> { { "old subscriber", null },{"new subscriber proxy",new MockRemoteSubscriber()} };
            var response = broker.GetOrCreateAProxy("new subscriber proxy");
            Assert.IsNotNull(response);
            Assert.AreEqual("It'smy party", response.GetHeartbeat().RootPath);
            Assert.AreEqual(2, broker.InitiatedSubScriberProxies.Count);
        }

        [Test]
        public void GetOrCreateAProxy_RegisteredButDead()
        {
            StoreType storeType = new StoreType { StoreName = "store1.xml", StoreTypeName = "IStore.Registration.FileSystemStore" };
            ComponentResolver componentResolver = new ComponentResolver();
            componentResolver.RegisterAll(new TestInstaller());
            Broker broker = new Broker(storeType, componentResolver);
            broker.InitiatedSubScriberProxies = new SortedList<string, ISubscriberProxy> { { "old subscriber", null }, { "new subscriber proxy", new MockRemoteSubscriber() } };
            Assert.IsNull(broker.GetOrCreateAProxy("old subscriber"));
        }

        [Test]
        public void DistributeMessageToSubscriber_SubscrberDead()
        {
            StoreType storeType = new StoreType { StoreName = "store1.xml", StoreTypeName = "IStore.Registration.FileSystemStore" };
            ComponentResolver componentResolver = new ComponentResolver();
            componentResolver.RegisterAll(new TestInstaller());
            Broker broker = new Broker(storeType, componentResolver);
            broker.InitiatedSubScriberProxies = new SortedList<string, ISubscriberProxy> { { "old subscriber", null }, { "new subscriber proxy", new MockRemoteSubscriber() } };
            Guid myGuid = Guid.NewGuid();
            broker._receivedMessages = new List<MessageWithDestinations> { new MessageWithDestinations { MessageId = myGuid, Destinations = new List<DestinationWithResult>() } };
            var deadEnd = new AddresedMessage
                {
                    DestinationUrl = "old subscriber",
                    ProcessedByDestination = true,
                    OriginalMessage = new PublisherMessage { MessageId = myGuid }
                };
            broker.DistributeMessageToSubscriber(deadEnd);
            Assert.False(deadEnd.ProcessedByDestination);
        }
        [Test]
        public void DistributeMessageToSubscriber_Ok()
        {
            StoreType storeType = new StoreType { StoreName = "store1.xml", StoreTypeName = "IStore.Registration.FileSystemStore" };
            ComponentResolver componentResolver = new ComponentResolver();
            componentResolver.RegisterAll(new TestInstaller());
            Broker broker = new Broker(storeType, componentResolver);
            broker.InitiatedSubScriberProxies = new SortedList<string, ISubscriberProxy> { { "old subscriber", null }, { "new subscriber proxy", new MockRemoteSubscriber() } };
            Guid myGuid = Guid.NewGuid();
            broker._receivedMessages = new List<MessageWithDestinations> { new MessageWithDestinations { MessageId = myGuid, Destinations = new List<DestinationWithResult>() } };
            var destination = new AddresedMessage
            {
                DestinationUrl = "new subscriber proxy",
                ProcessedByDestination = false,
                OriginalMessage = new PublisherMessage { MessageId = myGuid }
            };
            broker.DistributeMessageToSubscriber(destination);
            Assert.True(destination.ProcessedByDestination);
        }
        [Test]
        public void DistributeMessageToSubscriber_Exception()
        {
            StoreType storeType = new StoreType { StoreName = "store1.xml", StoreTypeName = "IStore.Registration.FileSystemStore" };
            ComponentResolver componentResolver = new ComponentResolver();
            componentResolver.RegisterAll(new TestInstaller());
            Broker broker = new Broker(storeType, componentResolver);
            broker.InitiatedSubScriberProxies = new SortedList<string, ISubscriberProxy> { { "old subscriber", null }, { "new subscriber proxy", new MockRemoteSubscriberNotPresent() } };
            Guid myGuid = Guid.NewGuid();
            broker._receivedMessages = new List<MessageWithDestinations>
                                           {new MessageWithDestinations {MessageId = myGuid,Destinations= new List<DestinationWithResult>()}};
            var destination = new AddresedMessage
            {
                DestinationUrl = "new subscriber proxy",
                ProcessedByDestination = false,
                OriginalMessage = new PublisherMessage { MessageId=myGuid}
            };
            broker.DistributeMessageToSubscriber(destination);
            Assert.False(destination.ProcessedByDestination);
        }

        [Test]
        public void DistributeMessageToAllAvailableSubscribers_TwoThreads_Ok()
        {
            StoreType storeType = new StoreType { StoreName = @"Data\storetestwsub.xml", StoreTypeName = "IStore.Registration.FileSystemStore" };
            ComponentResolver componentResolver = new ComponentResolver();
            componentResolver.RegisterAll(new TestInstaller());
            Broker broker = new Broker(storeType, componentResolver);
            broker.InitiatedSubScriberProxies = new SortedList<string, ISubscriberProxy> { { "old subscriber", new MockRemoteSubscriber() }, { "new subscriber proxy", new MockRemoteSubscriber() } };

            PublisherMessage message=new PublisherMessage{OperationType=OperationType.Insert,MessageId=Guid.NewGuid()};
            broker.ReceiveAndDistributeMessage(new ReceiveAndDistributeMessageRequest{PublisherMessage=message});
            Thread.Sleep(5000); //wait for both threads to finish before checking it
            
            Assert.False(broker.ListAllMessages().AvailableMessages[0].Destinations.Any(d=>!d.Processed));
            
        }
        #endregion

        #region Constructor and Initialization Related
        [Test]
        [ExpectedException(typeof(ComponentNotRegieteredException))]
        public void Broker_ComponentNotRegistered()
        {
            ComponentResolver ComponentResolver= new ComponentResolver();
            ComponentResolver.RegisterAll(new TestInstaller());
            Broker broker =
                new Broker(new StoreType {StoreName = "store.xml", StoreTypeName = "IStore.Registration.Wrong"},
                           ComponentResolver);
        }

        [Test]
        public void GetHeartBeat_ok()
        {
            StoreType storeType = new StoreType { StoreName = "store1.xml", StoreTypeName = "IStore.Registration.FileSystemStore" };
            ComponentResolver componentResolver = new ComponentResolver();
            componentResolver.RegisterAll(new AllStoresInstaller());
            Broker broker = new Broker(storeType, componentResolver);
            Assert.True(broker.GetHeartbeat().Status);
        }
        #endregion
    }
}
