using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MySynch.Common.Logging;
using MySynch.Common.Serialization;
using MySynch.Contracts.Messages;
using MySynch.Core.DataTypes;

namespace MySynch.Core.Publisher
{
    public static class OfflineChangesDetector
    {
        internal static SortedList<string, OperationType> GetOfflineChanges(SynchItem newRepository, string oldRepositoryFileName)
        {
            LoggingManager.Debug("Getting the offline changes based on file: " + oldRepositoryFileName);
            if (newRepository == null)
                throw new ArgumentNullException("currentRepository");
            if(string.IsNullOrEmpty(oldRepositoryFileName) || !File.Exists(oldRepositoryFileName))
                return new SortedList<string, OperationType>();

            var oldRepository = Serializer.DeserializeFromFile<SynchItem>(oldRepositoryFileName);
            if (oldRepository.Count == 0)
                return new SortedList<string, OperationType>();
            return GetDifferencesBetweenTrees(newRepository, (oldRepository.Count == 0) ? new SynchItem() : oldRepository[0]);
        }

        internal static SortedList<string, OperationType> GetDifferencesBetweenTrees(SynchItem newTree, SynchItem oldTree)
        {
            LoggingManager.Debug("Getting the differences between trees");
            List<SynchItemData> newTreeFlatten = SynchItemManager.FlattenTree(newTree);
            List<SynchItemData> oldTreeFlatten = SynchItemManager.FlattenTree(oldTree);
            SortedList<string, OperationType> result = new SortedList<string, OperationType>();
            if (newTree.Items == null)
            {
                if (oldTree.Items == null)
                    return new SortedList<string, OperationType>();
                else
                {
                    foreach (SynchItemData synchItemData in oldTreeFlatten)
                        result.Add(synchItemData.Identifier, OperationType.Delete);
                    return result;
                }
            }
            else
            {
                if (oldTree.Items == null)
                {
                    foreach (SynchItemData synchItemData in newTreeFlatten)
                        result.Add(synchItemData.Identifier, OperationType.Insert);
                    return result;
                }
            }
            if (newTree.SynchItemData.Identifier != oldTree.SynchItemData.Identifier)
                throw new PublisherSetupException(newTree.SynchItemData.Identifier, "The backup does not belong to the root folder");
            foreach (string key in newTreeFlatten.Except(oldTreeFlatten, new SynchItemDataEqualityComparer()).Select(o => o.Identifier))
                result.Add(key, OperationType.Insert);
            foreach (string key in newTreeFlatten.Join(oldTreeFlatten, n => n.Identifier, o => o.Identifier,
                                (n, o) =>
                                new { Identifier = n.Identifier, Name = n.Name, NewSize = n.Size, OldSize = o.Size }).
                Where(c => c.NewSize != c.OldSize).Select(c => c.Identifier))
                result.Add(key, OperationType.Update);

            foreach (string key in oldTreeFlatten.Except(newTreeFlatten, new SynchItemDataEqualityComparer()).Select(n => n.Identifier))
                result.Add(key, OperationType.Delete);
            LoggingManager.Debug("Differences calculated.");
            return result;
        }

        internal static void ForcePublishAllOfflineChanges(PushPublisher publisher,string oldRepositoryFileName, string rootFolder)
        {
            try
            {
                publisher.CurrentRepository = ItemDiscoverer.DiscoverFromFolder(rootFolder);
                var changesToSend = OfflineChangesDetector.GetOfflineChanges(publisher.CurrentRepository,
                                                         oldRepositoryFileName);
                foreach (var changeToSend in changesToSend)
                {
                    switch (changeToSend.Value)
                    {
                        case OperationType.Insert:
                            publisher.QueueInsert(changeToSend.Key);
                            break;
                        case OperationType.Update:
                            publisher.QueueUpdate(changeToSend.Key);
                            break;
                        case OperationType.Delete:
                            publisher.QueueDelete(changeToSend.Key);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                LoggingManager.LogMySynchSystemError(ex);  
            }
        }

    }
}
