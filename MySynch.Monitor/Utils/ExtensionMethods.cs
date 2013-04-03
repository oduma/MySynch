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
                                                                                  PackageId=mapChannel.PublisherInfo.CurrentPackage.Id,
                                                                                  PublisherPackageState=mapChannel.PublisherInfo.CurrentPackage.State,
                                                                                  MessagesProcessed=mapChannel.PublisherInfo.CurrentPackage.PackageMessages.AddToMessages(mapChannel.SubscriberInfo.CurrentPackage.PackageMessages,mapChannel.PublisherInfo.RootPath,mapChannel.SubscriberInfo.RootPath),
                                                                              MapChannelSubscriberTitle =
                                                                                  mapChannel.SubscriberInfo.InstanceName + ":" + mapChannel.SubscriberInfo.Port,
                                                                                  SubscriberStatus=mapChannel.SubscriberInfo.Status,
                                                                                  SubscriberRootPath=mapChannel.SubscriberInfo.RootPath,
                                                                                  SubscriberPackageState=mapChannel.SubscriberInfo.CurrentPackage.State
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
                        existingItem.PackageId = mapChannel.PublisherInfo.CurrentPackage.Id;
                        existingItem.PublisherPackageState = mapChannel.PublisherInfo.CurrentPackage.State;
                        existingItem.MessagesProcessed=
                            mapChannel.PublisherInfo.CurrentPackage.PackageMessages.AddToMessages(mapChannel.SubscriberInfo.CurrentPackage.PackageMessages,mapChannel.PublisherInfo.RootPath,mapChannel.SubscriberInfo.RootPath,
                            existingItem.MessagesProcessed);
                        existingItem.SubscriberPackageState = mapChannel.SubscriberInfo.CurrentPackage.State;
                    }
                }
            }
            return beforeImage;
        }

        internal static ObservableCollection<MessageViewModel> AddToMessages( this IEnumerable<FeedbackMessage> inCollection,IEnumerable<FeedbackMessage> subscriberProcessedMessages, string sourceRootPath, string destinationRootPath, ObservableCollection<MessageViewModel> beforeImage=null)
        {
            if (beforeImage == null)
                beforeImage = new ObservableCollection<MessageViewModel>();
            if (inCollection == null)
                return beforeImage;
            foreach (var message in inCollection)
            {
                var subscriberMessage =
                    subscriberProcessedMessages.FirstOrDefault(
                        s => s.AbsolutePath.Replace(destinationRootPath, sourceRootPath) == message.AbsolutePath);
                MessageViewModel messageViewModel = new MessageViewModel
                                                        {
                                                            RelativePath = message.AbsolutePath.Replace(sourceRootPath,""),
                                                            Done = (subscriberMessage==null)?false:subscriberMessage.Processed,
                                                            OperationType = message.OperationType
                                                        };
                if (!beforeImage.Contains(messageViewModel, new MessageViewModelEqualityComparer()))
                {
                    if (Application.Current == null)
                        beforeImage.Add(messageViewModel);
                    else
                        Application.Current.Dispatcher.Invoke((Action)(() => beforeImage.Add(messageViewModel)));
                }
                else
                {
                    var existingItem = beforeImage.FirstOrDefault(
                        b =>
                        b.RelativePath == messageViewModel.RelativePath);
                    if (existingItem != null)
                    {
                        existingItem.Done = messageViewModel.Done;
                        existingItem.OperationType = messageViewModel.OperationType;
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
