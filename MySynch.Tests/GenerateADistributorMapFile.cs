using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
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
            availableChannel.CopyStartegyName = "ICopyStrategy.Local";
            availableChannel.PublisherInfo= new PublisherInfo();
            availableChannel.PublisherInfo.MachineName = Environment.MachineName;
            availableChannel.PublisherInfo.PublisherInstanceName = "IPublisher.Local";
            availableChannel.Status = Status.NotChecked;
            availableChannel.SubscriberInfo= new SubscriberInfo();
            availableChannel.SubscriberInfo.MachineName = Environment.MachineName;
            availableChannel.SubscriberInfo.SubScriberName = "ISubscriber.Local";
            availableChannel.UniqueKey = availableChannel.PublisherInfo.MachineName + "to" +
                                         availableChannel.SubscriberInfo.MachineName;
            availableChannels.Add(availableChannel);

            XmlSerializer xmlSerializer= new XmlSerializer(typeof(List<AvailableChannel>));
            using (FileStream fs = new FileStream("distributormap.xml", FileMode.Create))
                xmlSerializer.Serialize(fs, availableChannels);

        }
    }
}
