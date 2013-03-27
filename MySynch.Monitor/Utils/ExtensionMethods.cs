using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
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
            if(inCollection==null)
                return beforeImage;
            foreach (var mapChannel in inCollection)
            {
                AvailableChannelViewModel availableChannelViewModel = new AvailableChannelViewModel
                                                                          {
                                                                              MapChannelPublisherTitle =
                                                                                  mapChannel.PublisherInfo.InstanceName + ":" + mapChannel.PublisherInfo.Port,
                                                                                  PublisherStatus=mapChannel.PublisherInfo.Status,
                                                                                  PublisherRootPath=mapChannel.PublisherInfo.RootPath,
                                                                                  PackagesAtPublisher=mapChannel.PublisherInfo.Packages.AddToPackages(),
                                                                              MapChannelSubscriberTitle =
                                                                                  mapChannel.SubscriberInfo.InstanceName + ":" + mapChannel.SubscriberInfo.Port,
                                                                                  SubscriberStatus=mapChannel.SubscriberInfo.Status,
                                                                                  SubscriberRootPath=mapChannel.SubscriberInfo.RootPath,
                                                                                  PackagesAtSubscriber=mapChannel.SubscriberInfo.Packages.AddToPackages()
                                                                          };
                if (!beforeImage.Contains(availableChannelViewModel, new AvailableChannelViewModelEqualityComparer()))
                {
                    if(Application.Current==null)
                        beforeImage.Add(availableChannelViewModel);
                    else
                        Application.Current.Dispatcher.Invoke((Action)(() => beforeImage.Add(availableChannelViewModel)));
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
                        existingItem.PackagesAtPublisher =
                            mapChannel.PublisherInfo.Packages.AddToPackages(existingItem.PackagesAtPublisher);
                        existingItem.PackagesAtSubscriber =
                            mapChannel.SubscriberInfo.Packages.AddToPackages(existingItem.PackagesAtSubscriber);
                    }

                }
            }
            return beforeImage;
        }

        internal static ObservableCollection<PackageViewModel> AddToPackages(this IEnumerable<Package> inCollection, ObservableCollection<PackageViewModel> beforeImage=null)
        {
            if (beforeImage == null)
                beforeImage = new ObservableCollection<PackageViewModel>();
            if(inCollection==null)
                return beforeImage;
            var previouslyRemoved = beforeImage.Where(b => b.PackageState == State.Done).ToList();
            foreach (var removedItem in previouslyRemoved)
            {
                if (Application.Current == null)
                {
                    beforeImage.Remove(removedItem);
                }
                else
                {
                    PackageViewModel item = removedItem;
                    Application.Current.Dispatcher.Invoke((Action)(() => beforeImage.Remove(item)));
                }
            }
            foreach (var package in inCollection)
            {
                PackageViewModel packageViewModel = new PackageViewModel {PackageId = package.Id, PackageState=package.State};
                if (!beforeImage.Contains(packageViewModel, new PackageViewModelEqualityComparer()))
                {
                    if (Application.Current == null)
                        beforeImage.Add(packageViewModel);
                    else
                        Application.Current.Dispatcher.Invoke((Action)(() => beforeImage.Add(packageViewModel)));
                }
                else
                {
                    var existingItem = beforeImage.FirstOrDefault(
                        b =>
                        b.PackageId == packageViewModel.PackageId);
                    if (existingItem != null)
                    {
                        existingItem.PackageState = packageViewModel.PackageState;
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
