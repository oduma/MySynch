﻿using System;
using System.Collections.Generic;
using System.Linq;
using MySynch.Contracts.Messages;

namespace MySynch.Core
{
    public partial class SynchItemManager
    {
        public static void AddItem(SynchItem topSynchItem, string absolutePathtoNewItem)
        {
            if (string.IsNullOrEmpty(absolutePathtoNewItem))
                return;
            if (topSynchItem == null)
                return;
            var parrent=GetItemLowestAvailableParrent(topSynchItem, absolutePathtoNewItem);
            if (parrent.Identifier == absolutePathtoNewItem)
                return;
            string[] restOfLevels = absolutePathtoNewItem.Replace(parrent.Identifier +"\\", "").Split(new char[] {'\\'});
            foreach (string level in restOfLevels)
            {
                var current = new SynchItem {Name = level, Identifier = parrent.Identifier + @"\" + level};
                if (parrent.Items == null)
                    parrent.Items=new List<SynchItem>();
                parrent.Items.Add(current);
                parrent = current;
            }
        }

        public static void UpdateExistingItem(SynchItem topSynchItem, string absolutePathtoNewItem)
        {
            return;
        }

        public static void DeleteItem(SynchItem topSynchItem, string absolutePathtoNewItem)
        {
            return;
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
                currentItem = list.FirstOrDefault(i => i.Identifier == currentLevel);
                if (currentItem == null)
                    return parentItem;
                parentItem = currentItem;
                list = parentItem.Items;
            }
            return parentItem;
        }


    }
}