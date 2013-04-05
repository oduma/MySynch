using System;
using System.Collections.ObjectModel;
using System.Linq;
using MySynch.Contracts.Messages;
using MySynch.Monitor.MVVM.ViewModels;
using MySynch.Monitor.Utils;
using NUnit.Framework;
using System.Collections.Generic;

namespace MySynch.MVVM.Tests
{
    [TestFixture]
    public class ExtentionMethodsTests
    {
        [Test]
        public void ConvertToChannels_Ok()
        {
            ObservableCollection<MapChannelViewModel> mapChannelViewModels=new ObservableCollection<MapChannelViewModel>();
            mapChannelViewModels.Add(new MapChannelViewModel{MapChannelPublisherTitle="IPublisher.Remote:1234",MapChannelSubscriberTitle="ISubscriber.Remote:1234"});
            mapChannelViewModels.Add(new MapChannelViewModel { MapChannelPublisherTitle = "IPublisher.Remote:5678", MapChannelSubscriberTitle = "ISubscriber.Remote:5678" });
            mapChannelViewModels.Add(new MapChannelViewModel { MapChannelPublisherTitle = "IPublisher.Remote:9012", MapChannelSubscriberTitle = "ISubscriber.Remote:9012" });
            mapChannelViewModels.Add(new MapChannelViewModel { MapChannelPublisherTitle = "IPublisher.Remote:3456", MapChannelSubscriberTitle = "ISubscriber.Remote:3456" });
            mapChannelViewModels.Add(new MapChannelViewModel { MapChannelPublisherTitle = "IPublisher.Remote:7890", MapChannelSubscriberTitle = "ISubscriber.Remote:7890" });
            var channels = mapChannelViewModels.ConvertToChannels();
            Assert.IsNotNull(channels);
            Assert.AreEqual(5,channels.Count());
            Assert.AreEqual(5,channels.Count(c=>c.PublisherInfo.InstanceName=="IPublisher.Remote" && c.PublisherInfo.Port>0));
            Assert.AreEqual(5, channels.Count(c => c.SubscriberInfo.InstanceName == "ISubscriber.Remote" && c.SubscriberInfo.Port > 0));
        }
        [Test]
        public void ConvertToChannels_PartNoPublishers()
        {
            ObservableCollection<MapChannelViewModel> mapChannelViewModels = new ObservableCollection<MapChannelViewModel>();
            mapChannelViewModels.Add(new MapChannelViewModel { MapChannelPublisherTitle = "IPublisher.Remote:1234", MapChannelSubscriberTitle = "ISubscriber.Remote:1234" });
            mapChannelViewModels.Add(new MapChannelViewModel { MapChannelPublisherTitle = "", MapChannelSubscriberTitle = "ISubscriber.Remote:5678" });
            mapChannelViewModels.Add(new MapChannelViewModel { MapChannelPublisherTitle = "IPublisher.Remote:9012", MapChannelSubscriberTitle = "ISubscriber.Remote:9012" });
            mapChannelViewModels.Add(new MapChannelViewModel { MapChannelPublisherTitle = null, MapChannelSubscriberTitle = "ISubscriber.Remote:3456" });
            mapChannelViewModels.Add(new MapChannelViewModel { MapChannelPublisherTitle = "IPublisher.Remote:7890", MapChannelSubscriberTitle = "ISubscriber.Remote:7890" });
            var channels = mapChannelViewModels.ConvertToChannels();
            Assert.IsNotNull(channels);
            Assert.AreEqual(3, channels.Count());
            Assert.AreEqual(3,
                            channels.Count(
                                c => c.PublisherInfo.InstanceName == "IPublisher.Remote" && c.PublisherInfo.Port > 0));
            Assert.AreEqual(3, channels.Count(c => c.SubscriberInfo.InstanceName == "ISubscriber.Remote" && c.SubscriberInfo.Port > 0));
        }
        [Test]
        public void ConvertToChannels_PartNoSubscribers()
        {
            ObservableCollection<MapChannelViewModel> mapChannelViewModels = new ObservableCollection<MapChannelViewModel>();
            mapChannelViewModels.Add(new MapChannelViewModel { MapChannelPublisherTitle = "IPublisher.Remote:1234", MapChannelSubscriberTitle = null });
            mapChannelViewModels.Add(new MapChannelViewModel { MapChannelPublisherTitle = "IPublisher.Remote:5678", MapChannelSubscriberTitle = "ISubscriber.Remote:5678" });
            mapChannelViewModels.Add(new MapChannelViewModel { MapChannelPublisherTitle = "IPublisher.Remote:9012", MapChannelSubscriberTitle = string.Empty });
            mapChannelViewModels.Add(new MapChannelViewModel { MapChannelPublisherTitle = "IPublisher.Remote:3456", MapChannelSubscriberTitle = "ISubscriber.Remote:3456" });
            mapChannelViewModels.Add(new MapChannelViewModel { MapChannelPublisherTitle = "IPublisher.Remote:7890", MapChannelSubscriberTitle = "ISubscriber.Remote:7890" });
            var channels = mapChannelViewModels.ConvertToChannels();
            Assert.IsNotNull(channels);
            Assert.AreEqual(3, channels.Count());
            Assert.AreEqual(3,
                            channels.Count(
                                c => c.PublisherInfo.InstanceName == "IPublisher.Remote" && c.PublisherInfo.Port > 0));
            Assert.AreEqual(3, channels.Count(c => c.SubscriberInfo.InstanceName == "ISubscriber.Remote" && c.SubscriberInfo.Port > 0));
        }
        [Test]
        public void ConvertToChannels_PartWrongPublisher()
        {
            ObservableCollection<MapChannelViewModel> mapChannelViewModels = new ObservableCollection<MapChannelViewModel>();
            mapChannelViewModels.Add(new MapChannelViewModel { MapChannelPublisherTitle = "IPublisher.Remote:1234:234", MapChannelSubscriberTitle = "ISubscriber.Remote:1234" });
            mapChannelViewModels.Add(new MapChannelViewModel { MapChannelPublisherTitle = "IPublisher.Remote:5678", MapChannelSubscriberTitle = "ISubscriber.Remote:5678" });
            mapChannelViewModels.Add(new MapChannelViewModel { MapChannelPublisherTitle = "IPublisher.Remote9012", MapChannelSubscriberTitle = "ISubscriber.Remote:9012" });
            mapChannelViewModels.Add(new MapChannelViewModel { MapChannelPublisherTitle = "IPublisher.Remote:3456", MapChannelSubscriberTitle = "ISubscriber.Remote:3456" });
            mapChannelViewModels.Add(new MapChannelViewModel { MapChannelPublisherTitle = "IPublisher.Remote:7890", MapChannelSubscriberTitle = "ISubscriber.Remote:7890" });
            var channels = mapChannelViewModels.ConvertToChannels();
            Assert.IsNotNull(channels);
            Assert.AreEqual(3, channels.Count());
            Assert.AreEqual(3,
                            channels.Count(
                                c => c.PublisherInfo.InstanceName == "IPublisher.Remote" && c.PublisherInfo.Port > 0));
            Assert.AreEqual(3, channels.Count(c => c.SubscriberInfo.InstanceName == "ISubscriber.Remote" && c.SubscriberInfo.Port > 0));
        }
        [Test]
        public void ConvertToChannels_PartWrongSubscriber()
        {
            ObservableCollection<MapChannelViewModel> mapChannelViewModels = new ObservableCollection<MapChannelViewModel>();
            mapChannelViewModels.Add(new MapChannelViewModel { MapChannelPublisherTitle = "IPublisher.Remote:1234", MapChannelSubscriberTitle = "ISubscriber.Remote:1234" });
            mapChannelViewModels.Add(new MapChannelViewModel { MapChannelPublisherTitle = "IPublisher.Remote:5678", MapChannelSubscriberTitle = ":5678" });
            mapChannelViewModels.Add(new MapChannelViewModel { MapChannelPublisherTitle = "IPublisher.Remote:9012", MapChannelSubscriberTitle = "ISubscriber.Remote:9012" });
            mapChannelViewModels.Add(new MapChannelViewModel { MapChannelPublisherTitle = "IPublisher.Remote:3456", MapChannelSubscriberTitle = "ISubscriber:Remote:3456" });
            mapChannelViewModels.Add(new MapChannelViewModel { MapChannelPublisherTitle = "IPublisher.Remote:7890", MapChannelSubscriberTitle = "ISubscriber.Remote:7890" });
            var channels = mapChannelViewModels.ConvertToChannels();
            Assert.IsNotNull(channels);
            Assert.AreEqual(3, channels.Count());
            Assert.AreEqual(3,
                            channels.Count(
                                c => c.PublisherInfo.InstanceName == "IPublisher.Remote" && c.PublisherInfo.Port > 0));
            Assert.AreEqual(3, channels.Count(c => c.SubscriberInfo.InstanceName == "ISubscriber.Remote" && c.SubscriberInfo.Port > 0));
        }
        [Test]
        public void ConvertToChannels_PartWrongPublisherPort()
        {
            ObservableCollection<MapChannelViewModel> mapChannelViewModels = new ObservableCollection<MapChannelViewModel>();
            mapChannelViewModels.Add(new MapChannelViewModel { MapChannelPublisherTitle = "IPublisher.Remote:abc", MapChannelSubscriberTitle = "ISubscriber.Remote:1234" });
            mapChannelViewModels.Add(new MapChannelViewModel { MapChannelPublisherTitle = "IPublisher.Remote:56789078878778", MapChannelSubscriberTitle = "ISubscriber.Remote:5678" });
            mapChannelViewModels.Add(new MapChannelViewModel { MapChannelPublisherTitle = "IPublisher.Remote:9012", MapChannelSubscriberTitle = "ISubscriber.Remote:9012" });
            mapChannelViewModels.Add(new MapChannelViewModel { MapChannelPublisherTitle = "IPublisher.Remote:3456", MapChannelSubscriberTitle = "ISubscriber.Remote:3456" });
            mapChannelViewModels.Add(new MapChannelViewModel { MapChannelPublisherTitle = "IPublisher.Remote:7890", MapChannelSubscriberTitle = "ISubscriber.Remote:7890" });
            var channels = mapChannelViewModels.ConvertToChannels();
            Assert.IsNotNull(channels);
            Assert.AreEqual(3, channels.Count());
            Assert.AreEqual(3,
                            channels.Count(
                                c => c.PublisherInfo.InstanceName == "IPublisher.Remote" && c.PublisherInfo.Port > 0));
            Assert.AreEqual(3, channels.Count(c => c.SubscriberInfo.InstanceName == "ISubscriber.Remote" && c.SubscriberInfo.Port > 0));
        }
        [Test]
        public void ConvertToChannels_PartWrongSubscriberPort()
        {
            ObservableCollection<MapChannelViewModel> mapChannelViewModels = new ObservableCollection<MapChannelViewModel>();
            mapChannelViewModels.Add(new MapChannelViewModel { MapChannelPublisherTitle = "IPublisher.Remote:1234", MapChannelSubscriberTitle = "ISubscriber.Remote:1234" });
            mapChannelViewModels.Add(new MapChannelViewModel { MapChannelPublisherTitle = "IPublisher.Remote:5678", MapChannelSubscriberTitle = "ISubscriber.Remote: " });
            mapChannelViewModels.Add(new MapChannelViewModel { MapChannelPublisherTitle = "IPublisher.Remote:9012", MapChannelSubscriberTitle = "ISubscriber.Remote:9012" });
            mapChannelViewModels.Add(new MapChannelViewModel { MapChannelPublisherTitle = "IPublisher.Remote:3456", MapChannelSubscriberTitle = "ISubscriber.Remote:" });
            mapChannelViewModels.Add(new MapChannelViewModel { MapChannelPublisherTitle = "IPublisher.Remote:7890", MapChannelSubscriberTitle = "ISubscriber.Remote:7890" });
            var channels = mapChannelViewModels.ConvertToChannels();
            Assert.IsNotNull(channels);
            Assert.AreEqual(3, channels.Count());
            Assert.AreEqual(3,
                            channels.Count(
                                c => c.PublisherInfo.InstanceName == "IPublisher.Remote" && c.PublisherInfo.Port > 0));
            Assert.AreEqual(3, channels.Count(c => c.SubscriberInfo.InstanceName == "ISubscriber.Remote" && c.SubscriberInfo.Port > 0));
        }

        [Test]
        public void AddToChannels_Ok()
        {
            var beforeImage = new ObservableCollection<AvailableChannelViewModel>
                                  {
                                      new AvailableChannelViewModel
                                          {
                                              MapChannelPublisherTitle = "IPublisher.Remote:3585",
                                              PublisherStatus=Status.OfflineTemporary,
                                              PublisherRootPath="path1",
                                              MapChannelSubscriberTitle = "ISubscriber.Remote:2344",
                                              SubscriberStatus=Status.Ok,
                                              SubscriberRootPath="path2"
                                          }
                                  };
            var observableChannels =
                (new[]
                     {
                         new MapChannel
                             {
                                 PublisherInfo= new MapChannelComponent{InstanceName="IPublisher.Remote",Port=3586,Status=Status.Ok,RootPath="path3"},
                                 SubscriberInfo=new MapChannelComponent{InstanceName="ISubscriber.Remote",Port=2345,Status=Status.Ok,RootPath="path4"}
                             }
                     }).AddToChannels(beforeImage);
            Assert.IsNotNull(observableChannels);
            Assert.AreEqual(2,observableChannels.Count);
            Assert.AreEqual(1,observableChannels.Count(o=>o.MapChannelPublisherTitle=="IPublisher.Remote:3586" && o.MapChannelSubscriberTitle=="ISubscriber.Remote:2345"));
            Assert.AreEqual(2,observableChannels.Count(o=>o.SubscriberStatus==Status.Ok));
            Assert.AreEqual(1,observableChannels.Count(o=>o.PublisherStatus==Status.Ok));
            Assert.AreEqual(0,observableChannels.Count(o=>string.IsNullOrEmpty(o.PublisherRootPath)));
            Assert.AreEqual(0,observableChannels.Count(o=>string.IsNullOrEmpty(o.SubscriberRootPath)));
        }

        [Test]
        public void AddToChannels_EmptyInCollection()
        {
            var beforeImage = new ObservableCollection<AvailableChannelViewModel>
                                  {
                                      new AvailableChannelViewModel
                                          {
                                              MapChannelPublisherTitle = "IPublisher.Remote:3585",
                                              PublisherStatus=Status.OfflinePermanent,
                                              PublisherRootPath="path1",
                                              MapChannelSubscriberTitle = "ISubscriber.Remote:2344",
                                              SubscriberStatus=Status.Ok,
                                              SubscriberRootPath="path2"
                                          }
                                  };
            var observableChannels =
                (new List<MapChannel>()).AddToChannels(beforeImage);
            Assert.IsNotNull(observableChannels);
            Assert.AreEqual(1, observableChannels.Count);
            Assert.AreEqual(1, observableChannels.Count(o => o.MapChannelPublisherTitle == "IPublisher.Remote:3585" && o.MapChannelSubscriberTitle == "ISubscriber.Remote:2344"));
            Assert.AreEqual(Status.OfflinePermanent,observableChannels[0].PublisherStatus);
            Assert.AreEqual(Status.Ok,observableChannels[0].SubscriberStatus);
            Assert.AreEqual(0, observableChannels.Count(o => string.IsNullOrEmpty(o.PublisherRootPath)));
            Assert.AreEqual(0, observableChannels.Count(o => string.IsNullOrEmpty(o.SubscriberRootPath)));
        }

        [Test]
        public void AddToChannels_NothingNewToAdd()
        {
            var beforeImage = new ObservableCollection<AvailableChannelViewModel>
                                  {
                                      new AvailableChannelViewModel
                                          {
                                              MapChannelPublisherTitle = "IPublisher.Remote:3586",
                                              PublisherStatus=Status.OfflinePermanent,
                                              MapChannelSubscriberTitle = "ISubscriber.Remote:2345",
                                              SubscriberStatus=Status.OfflineTemporary
                                          }
                                  };
            var observableChannels =
                (new[]
                     {
                         new MapChannel
                             {
                                 PublisherInfo= new MapChannelComponent{InstanceName="IPublisher.Remote",Port=3586, Status=Status.OfflineTemporary},
                                 SubscriberInfo=new MapChannelComponent{InstanceName="ISubscriber.Remote",Port=2345, Status=Status.Ok}
                             }
                     }).AddToChannels(beforeImage);
            Assert.IsNotNull(observableChannels);
            Assert.AreEqual(1, observableChannels.Count);
            Assert.AreEqual(1, observableChannels.Count(o => o.MapChannelPublisherTitle == "IPublisher.Remote:3586" && o.MapChannelSubscriberTitle == "ISubscriber.Remote:2345"));
            Assert.AreEqual(Status.OfflineTemporary,observableChannels[0].PublisherStatus);
            Assert.AreEqual(Status.Ok,observableChannels[0].SubscriberStatus);
        }

        [Test]
        public void AddToChannels_NoBeforeImage()
        {
            
            var observableChannels =
                (new[]
                     {
                         new MapChannel
                             {
                                 PublisherInfo= new MapChannelComponent{InstanceName="IPublisher.Remote",Port=3586, Status=Status.Ok},
                                 SubscriberInfo=new MapChannelComponent{InstanceName="ISubscriber.Remote",Port=2345, Status=Status.OfflineTemporary}
                             }
                     }).AddToChannels(null);
            Assert.IsNotNull(observableChannels);
            Assert.AreEqual(1, observableChannels.Count);
            Assert.AreEqual(1, observableChannels.Count(o => o.MapChannelPublisherTitle == "IPublisher.Remote:3586" && o.MapChannelSubscriberTitle == "ISubscriber.Remote:2345"));
            Assert.AreEqual(Status.OfflineTemporary,observableChannels[0].SubscriberStatus);
            Assert.AreEqual(Status.Ok,observableChannels[0].PublisherStatus);

        }

        [Test]
        public void AddToMessages_Ok()
        {
            var beforeImage = new ObservableCollection<MessageViewModel>
                                  {
                                      new MessageViewModel
                                          {
                                              RelativePath = "old item"
                                          }
                                  };
            var observableMessages =
                (new[]
                     {
                         new FeedbackMessage
                             {
                                 AbsolutePath= "new item",
                                 OperationType=OperationType.Insert,
                                 Processed=true
                             }
                     }).AddToMessages(null,"","",beforeImage);
            Assert.IsNotNull(observableMessages);
            Assert.AreEqual(2, observableMessages.Count);
            Assert.AreNotEqual(observableMessages[0].RelativePath, observableMessages[1].RelativePath);
        }

        [Test]
        public void AddToMessages_EmptyInCollection()
        {
            var beforeImage = new ObservableCollection<MessageViewModel>
                                  {
                                      new MessageViewModel
                                          {
RelativePath="old item"                                          }
                                  };
            var messageViewModels =
                (new List<FeedbackMessage>()).AddToMessages(null,"","",beforeImage);
            Assert.IsNotNull(messageViewModels);
            Assert.AreEqual(1, messageViewModels.Count);
        }

        [Test]
        public void AddToMessages_NothingNewToAdd()
        {
            var beforeImage = new ObservableCollection<MessageViewModel>
                                  {
                                      new MessageViewModel
                                          {
                                              RelativePath="old item",
                                              OperationType=OperationType.Insert,
                                              Done=false
                                          }
                                  };
            var observableMessages =
                (new[]
                     {
                         new FeedbackMessage
                             {
                                 AbsolutePath="old item",
                                 OperationType=OperationType.Update,
                                 Processed=false
                             }
                     }).AddToMessages(new[] { new FeedbackMessage
                             {
                                 AbsolutePath="old item",
                                 OperationType=OperationType.Update,
                                 Processed=true
                             }}, "", "", beforeImage);
            Assert.IsNotNull(observableMessages);
            Assert.AreEqual(1, observableMessages.Count);
            Assert.True(observableMessages[0].Done);
        }

        [Test]
        public void AddToMessages_NoBeforeImage()
        {

            var observableMessages =
                (new[]
                     {
                         new FeedbackMessage
                             {
                                 AbsolutePath= "new item",
                                 OperationType=OperationType.Insert,
                                 Processed=true
                             }
                     }).AddToMessages(null,"","");
            Assert.IsNotNull(observableMessages);
            Assert.AreEqual(1, observableMessages.Count);

        }

    }
}
