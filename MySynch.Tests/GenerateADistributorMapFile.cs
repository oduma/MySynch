using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using MySynch.Contracts.Messages;
using MySynch.Core;
using MySynch.Core.DataTypes;
using NUnit.Framework;

namespace MySynch.Tests
{
    [TestFixture]
    public class GenerateADistributorMapFile
    {
        [Test]
        public void Generate()
        {
            List<AvailableChannel> availableChannels=new List<AvailableChannel>();
            AvailableChannel availableChannel= new AvailableChannel();
            availableChannel.PublisherInfo= new PublisherInfo();
            availableChannel.PublisherInfo.Port = 0;
            availableChannel.PublisherInfo.InstanceName = "IPublisher.Local";
            availableChannel.Status = Status.NotChecked;
            availableChannel.SubscriberInfo= new SubscriberInfo();
            availableChannel.SubscriberInfo.Port = 0;
            availableChannel.SubscriberInfo.InstanceName = "ISubscriber.Local";
            availableChannel.UniqueKey = availableChannel.PublisherInfo.Port + "to" +
                                         availableChannel.SubscriberInfo.Port;
            availableChannels.Add(availableChannel);

            XmlSerializer xmlSerializer= new XmlSerializer(typeof(List<AvailableChannel>));
            using (FileStream fs = new FileStream("distributormap.xml", FileMode.Create))
                xmlSerializer.Serialize(fs, availableChannels);

        }
    }
}
