using System;
using System.Collections.Generic;
using System.Linq;
using MySynch.Contracts.Messages;
using MySynch.Core.DataTypes;

namespace MySynch.Core
{
    public partial class SynchItemManager
    {
        public static void AddItem(SynchItem topSynchItem, string absolutePathtoNewItem)
        {
            if (string.IsNullOrEmpty(absolutePathtoNewItem))
                return;
            if (topSynchItem == null || topSynchItem.Items==null)
                return;
            var parrent=GetItemLowestAvailableParrent(topSynchItem, absolutePathtoNewItem);
            if (parrent.SynchItemData.Identifier == absolutePathtoNewItem)
                return;
            AddNewItemTree(parrent, absolutePathtoNewItem);
        }

        private static void AddNewItemTree(SynchItem parrent, string absolutePathtoNewItem)
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
        }

        public static void UpdateExistingItem(SynchItem topSynchItem, string absolutePathtoNewItem)
        {
            if (string.IsNullOrEmpty(absolutePathtoNewItem))
                return;
            if (topSynchItem == null || topSynchItem.Items==null)
                return;
            var parrent=GetItemLowestAvailableParrent(topSynchItem, absolutePathtoNewItem);
            if (parrent.SynchItemData.Identifier == absolutePathtoNewItem)
            {
                if (!parrent.SynchItemData.Changed)
                    parrent.SynchItemData.Changed = true;
                return;
            }
            AddNewItemTree(parrent,absolutePathtoNewItem);
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


        public static List<SynchItemData> FlattenTree(SynchItem newTree)
        {
            throw new NotImplementedException();
        }
    }
}
