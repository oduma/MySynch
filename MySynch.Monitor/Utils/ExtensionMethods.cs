using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MySynch.Contracts.Messages;
using MySynch.Core.DataTypes;
using MySynch.Monitor.MVVM.ViewModels;

namespace MySynch.Monitor.Utils
{
    public static class ExtensionMethods
    {
        internal static IEnumerable<AvailableChannel> ConvertToChannels(this ObservableCollection<MapChannelViewModel> inCollection)
        {
            foreach (var inElement in inCollection)
            {
                if (!string.IsNullOrEmpty(inElement.MapChannelPublisherTitle)
     && !string.IsNullOrEmpty(inElement.MapChannelSubscriberTitle))
                {

                    var publisherParts = inElement.MapChannelPublisherTitle.Split(new char[] { ':' });
                    if (publisherParts.Count() == 2 && !string.IsNullOrEmpty(publisherParts[0]))
                    {

                        int publisherPort;
                        if (int.TryParse(publisherParts[1], out publisherPort))
                        {
                            var subscriberParts = inElement.MapChannelSubscriberTitle.Split(new char[] { ':' });
                            if (subscriberParts.Count() == 2 && !string.IsNullOrEmpty(subscriberParts[0]))
                            {
                                int subscriberPort;
                                if (int.TryParse(subscriberParts[1], out subscriberPort))
                                {
                                    yield return new AvailableChannel
                                        {
                                            PublisherInfo =
                                                new MapChannelComponent
                                                    {
                                                        InstanceName = publisherParts[0],
                                                        Port = publisherPort
                                                    },
                                            SubscriberInfo = new MapChannelComponent
                                                                 {
                                                                     InstanceName = subscriberParts[0],
                                                                     Port = subscriberPort
                                                                 }
                                        };
                                }
                            }
                        }
                    }
                }
            }
        }

        internal static ObservableCollection<AvailableChannelViewModel> AddToChannels(this IEnumerable<MapChannel> inCollection, ObservableCollection<AvailableChannelViewModel> beforeImage)
        {
            foreach (var mapChannel in inCollection)
            {
                AvailableChannelViewModel availableChannelViewModel = new AvailableChannelViewModel
                                                                          {
                                                                              MapChannelPublisherTitle =
                                                                                  mapChannel.PublisherInfo.InstanceName,
                                                                              MapChannelSubscriberTitle =
                                                                                  mapChannel.SubscriberInfo.InstanceName
                                                                          };
                if(beforeImage.Contains())
            }
        }

        internal static IEnumerable<string> ShiftLeft(this string[] source, int step)
        {
            for (int i = step; i < source.Count(); i++)
                yield return source[i];
        }
    }
}
