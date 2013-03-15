using System.Collections.ObjectModel;
using System.Linq;
using MySynch.Monitor.MVVM.ViewModels;
using MySynch.Monitor.Utils;
using NUnit.Framework;

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
    }
}
