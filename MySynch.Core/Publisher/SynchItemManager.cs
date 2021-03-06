﻿using System.Collections.Generic;
using System.Linq;
using MySynch.Core.DataTypes;

namespace MySynch.Core.Publisher
{
    public static class SynchItemManager
    {
        public static void AddItem(SynchItem topSynchItem, string absolutePathtoNewItem, long size)
        {
            if (string.IsNullOrEmpty(absolutePathtoNewItem))
                return;
            if (topSynchItem == null || topSynchItem.Items==null)
                return;
            var parrent=GetItemLowestAvailableParrent(topSynchItem, absolutePathtoNewItem);
            if (parrent.SynchItemData.Identifier == absolutePathtoNewItem)
                return;
            AddNewItemTree(parrent, absolutePathtoNewItem,size);
        }

        private static void AddNewItemTree(SynchItem parrent, string absolutePathtoNewItem,long size)
        {
            string[] restOfLevels = absolutePathtoNewItem.Replace(parrent.SynchItemData.Identifier +"\\", "").Split(new char[] {'\\'});
            foreach (string level in restOfLevels)
            {
                var current = new SynchItem
                                  {
                                      SynchItemData =
                                          new SynchItemData
                                              {Name = level, Identifier = parrent.SynchItemData.Identifier + @"\" + level}
                                  };
                if (parrent.Items == null)
                    parrent.Items=new List<SynchItem>();
                parrent.Items.Add(current);
                parrent = current;
            }
            if (parrent.Items==null || parrent.Items.Count == 0)
                parrent.SynchItemData.Size = size;
        }

        public static void UpdateExistingItem(SynchItem topSynchItem, string absolutePathtoNewItem, long size)
        {
            if (string.IsNullOrEmpty(absolutePathtoNewItem))
                return;
            if (topSynchItem == null || topSynchItem.Items==null)
                return;
            var parrent=GetItemLowestAvailableParrent(topSynchItem, absolutePathtoNewItem);
            if (parrent.SynchItemData.Identifier == absolutePathtoNewItem)
            {
                parrent.SynchItemData.Size = size;
                return;
            }
            AddNewItemTree(parrent,absolutePathtoNewItem, size);
        }

        public static void DeleteItem(SynchItem topSynchItem, string absolutePathtoNewItem)
        {
            if (string.IsNullOrEmpty(absolutePathtoNewItem))
                return;
            if (topSynchItem == null || topSynchItem.Items==null)
                return;
            var currentItem = GetItemLowestAvailableParrent(topSynchItem, absolutePathtoNewItem);
            if (currentItem.SynchItemData.Identifier == absolutePathtoNewItem)
            {
                var parenttem = GetParentItem(new List<SynchItem> {topSynchItem}, absolutePathtoNewItem);
                parenttem.Items.Remove(currentItem);
                return;
            }
        }

        internal static SynchItem GetItemLowestAvailableParrent(SynchItem topSynchItem, string itemId)
        {
            string[] levels = itemId.Split(new char[] { '\\' });

            SynchItem parentItem = topSynchItem;
            SynchItem currentItem = topSynchItem;
            string currentLevel = string.Empty;
            var list = new List<SynchItem> {topSynchItem};
            foreach (string level in levels)
            {
                currentLevel = (string.IsNullOrEmpty(currentLevel)) ? level : string.Format("{0}\\{1}", currentLevel, level);
                currentItem = list.FirstOrDefault(i => i.SynchItemData.Identifier == currentLevel);
                if (currentItem == null)
                    return parentItem;
                parentItem = currentItem;
                list = parentItem.Items;
            }
            return parentItem;
        }

        public static List<SynchItemData> FlattenTree(SynchItem tree)
        {
            List<SynchItemData> synchItems=new List<SynchItemData>();
            if (tree.Items == null || tree.Items.Count==0)
            {
                synchItems.Add(tree.SynchItemData);
                return synchItems;
            }
            foreach(SynchItem synchItem in tree.Items)
                synchItems.AddRange(FlattenTree(synchItem));
            return synchItems;
        }

        private static SynchItem GetParentItem(List<SynchItem> list, string Identifier)
        {
            string[] levels = Identifier.Split(new char[] { '\\' });
            if (levels.Count() <= 1)
                return null;
            string parentIdentifier = string.Empty;
            for (int i = 0; i < levels.Count() - 1; i++)
                parentIdentifier = (string.IsNullOrEmpty(parentIdentifier)) ? levels[i] : string.Format("{0}\\{1}", parentIdentifier, levels[i]);

            return GetItem(list, parentIdentifier);
        }


        private static SynchItem GetItem(List<SynchItem> list, string itemId)
        {
            string[] levels = itemId.Split(new char[] { '\\' });

            SynchItem parentItem = null;
            string currentLevel = string.Empty;
            foreach (string level in levels)
            {
                if (list == null)
                    return null;
                currentLevel = (string.IsNullOrEmpty(currentLevel)) ? level : string.Format("{0}\\{1}", currentLevel, level);
                parentItem = list.FirstOrDefault(i => i.SynchItemData.Identifier == currentLevel);
                if (parentItem == null)
                    return null;
                list = parentItem.Items;
            }
            return parentItem;
        }


    }
}
