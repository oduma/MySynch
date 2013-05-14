﻿using System.Collections.Generic;
using System.Linq;
using MySynch.Common.Serialization;
using MySynch.Contracts.Messages;
using MySynch.Core;
using MySynch.Core.Broker;
using MySynch.Tests.Stubs;
using NUnit.Framework;

namespace MySynch.Tests
{
    [TestFixture]
    public class BrokerTests
    {
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
            MySynchComponentResolver componentResolver = new MySynchComponentResolver();
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
            MySynchComponentResolver componentResolver = new MySynchComponentResolver();
            componentResolver.RegisterAll(new AllStoresInstaller());
            Broker broker = new Broker(storeType, componentResolver);
            AttachRequest request = new AttachRequest
            {
                RegistrationRequest =
                    new Registration
                    {
                        MessageMethod = "Message Method 1",
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
            MySynchComponentResolver componentResolver = new MySynchComponentResolver();
            componentResolver.RegisterAll(new AllStoresInstaller());
            Broker broker = new Broker(storeType, componentResolver);
            AttachRequest request = new AttachRequest
            {
                RegistrationRequest =
                    new Registration
                    {
                        MessageMethod = "Message Method 1",
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
            request = new AttachRequest
            {
                RegistrationRequest =
                    new Registration
                    {
                        MessageMethod = "Message Method 2",
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
            Assert.True(resonse.RegisteredOk);
        }

        [Test]
        public void Attach_Error()
        {
            StoreType storeType = new StoreType { StoreName = "store1.xml", StoreTypeName = "IStore.Registration.DeffectiveStore" };
            MySynchComponentResolver componentResolver = new MySynchComponentResolver();
            componentResolver.RegisterAll(new AllStoresInstaller());
            Broker broker = new Broker(storeType, componentResolver);
            AttachRequest request = new AttachRequest
            {
                RegistrationRequest =
                    new Registration
                    {
                        MessageMethod = "Message Method 1",
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
            registrations.RemoveRegistration("my reg 2");
            Assert.AreEqual(1, registrations.Count);
        }

        [Test]
        public void RemoveRegistration_Ok()
        {
            List<Registration> registrations = new List<Registration> { new Registration { ServiceUrl = "my reg 1" }, new Registration { ServiceUrl = "my reg 2" } };
            registrations.RemoveRegistration("my reg 2");
            Assert.AreEqual(1, registrations.Count);
        }
        [Test]
        public void RemoveRegistration_OnNewList()
        {
            List<Registration> registrations = new List<Registration>();
            registrations.RemoveRegistration("my reg 2");
            Assert.IsNotNull(registrations);
            Assert.AreEqual(0, registrations.Count);
        }

        [Test]
        public void RemoveRegistration_OnNull()
        {
            List<Registration> registrations = null;
            var resultRegistrations = registrations.RemoveRegistration("my reg 2");
            Assert.IsNotNull(resultRegistrations);
            Assert.AreEqual(0, resultRegistrations.Count);
        }

        [Test]
        public void Detach_No_ValidRegistration()
        {
            StoreType storeType = new StoreType { StoreName = "store1.xml", StoreTypeName = "IStore.Registration.FileSystemStore" };
            MySynchComponentResolver componentResolver = new MySynchComponentResolver();
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
            MySynchComponentResolver componentResolver = new MySynchComponentResolver();
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
            MySynchComponentResolver componentResolver = new MySynchComponentResolver();
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
            MySynchComponentResolver componentResolver = new MySynchComponentResolver();
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
            MySynchComponentResolver componentResolver = new MySynchComponentResolver();
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
            MySynchComponentResolver componentResolver = new MySynchComponentResolver();
            componentResolver.RegisterAll(new AllStoresInstaller());
            Broker broker = new Broker(storeType, componentResolver);
            AttachRequest request = new AttachRequest
            {
                RegistrationRequest =
                    new Registration
                    {
                        MessageMethod = "Message Method 1",
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
            MySynchComponentResolver componentResolver = new MySynchComponentResolver();
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
            MySynchComponentResolver componentResolver = new MySynchComponentResolver();
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
            MySynchComponentResolver componentResolver = new MySynchComponentResolver();
            componentResolver.RegisterAll(new AllStoresInstaller());
            Broker broker = new Broker(storeType, componentResolver);
            var response = broker.ListAllRegistrations();
            Assert.IsNotNull(response.Registrations);
            Assert.AreEqual(0, response.Registrations.Count);
        }

        #endregion

        [Test]
        public void ReceiveAndDistributeMessage_Ok()
        {
            StoreType storeType = new StoreType { StoreName = "store1.xml", StoreTypeName = "IStore.Registration.FileSystemStore" };
            MySynchComponentResolver componentResolver = new MySynchComponentResolver();
            componentResolver.RegisterAll(new AllStoresInstaller());
            Broker broker = new Broker(storeType, componentResolver);
            AttachRequest request = new AttachRequest
            {
                RegistrationRequest =
                    new Registration
                    {
                        MessageMethod = "Message Method 1",
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
            Assert.Fail();
        }
    }
}
