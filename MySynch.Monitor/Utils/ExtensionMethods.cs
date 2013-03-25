﻿using System;
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
            if(beforeImage==null)
                beforeImage=new ObservableCollection<AvailableChannelViewModel>();
            foreach (var mapChannel in inCollection)
            {
                AvailableChannelViewModel availableChannelViewModel = new AvailableChannelViewModel
                                                                          {
                                                                              MapChannelPublisherTitle =
                                                                                  mapChannel.PublisherInfo.InstanceName + ":" + mapChannel.PublisherInfo.Port,
                                                                                  PublisherStatus=mapChannel.PublisherInfo.Status,
                                                                                  PublisherRootPath=mapChannel.PublisherInfo.RootPath,
                                                                              MapChannelSubscriberTitle =
                                                                                  mapChannel.SubscriberInfo.InstanceName + ":" + mapChannel.SubscriberInfo.Port,
                                                                                  SubscriberStatus=mapChannel.SubscriberInfo.Status,
                                                                                  SubscriberRootPath=mapChannel.SubscriberInfo.RootPath
                                                                          };
                if (!beforeImage.Contains(availableChannelViewModel, new AvailableChannelViewModelEqualityComparer()))
                {
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => beforeImage.Add(availableChannelViewModel)));
                }
                else
                {
                    var existingItem = beforeImage.FirstOrDefault(
                        b =>
                        b.MapChannelPublisherTitle ==
                        mapChannel.PublisherInfo.InstanceName + ":" + mapChannel.PublisherInfo.Port
                        && b.MapChannelSubscriberTitle==mapChannel.SubscriberInfo.InstanceName + ":" +mapChannel.SubscriberInfo.Port);
                    if (existingItem != null)
                    {
                        existingItem.PublisherStatus = mapChannel.PublisherInfo.Status;
                        existingItem.SubscriberStatus = mapChannel.SubscriberInfo.Status;
                        existingItem.PublisherRootPath = mapChannel.PublisherInfo.RootPath;
                        existingItem.SubscriberRootPath = mapChannel.SubscriberInfo.RootPath;
                    }

                }
            }
            return beforeImage;
        }

        internal static IEnumerable<string> ShiftLeft(this string[] source, int step)
        {
            for (int i = step; i < source.Count(); i++)
                yield return source[i];
        }
    }
}
