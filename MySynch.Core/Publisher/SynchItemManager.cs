using System;
using System.Collections.Generic;
using System.Linq;
using MySynch.Core.DataTypes;

namespace MySynch.Core.Publisher
{
    public static class SynchItemManager
    {
        public static bool AddItem(SynchItem topSynchItem, string absolutePathtoNewItem, long size)
        {
            if (string.IsNullOrEmpty(absolutePathtoNewItem))
                return false;
            if (topSynchItem == null || topSynchItem.Items==null)
                return false;
            var parrent=GetItemLowestAvailableParrent(topSynchItem, absolutePathtoNewItem);
            if (parrent.SynchItemData.Identifier == absolutePathtoNewItem)
                return false;
            return AddNewItemTree(parrent, absolutePathtoNewItem,size);
        }

        private static bool AddNewItemTree(SynchItem parrent, string absolutePathtoNewItem,long size)
        {
            string[] restOfLevels = absolutePathtoNewItem.Replace(parrent.SynchItemData.Identifier +"\\", "").Split(new char[] {'\\'});
            if (parrent.Items.Any(i => i.SynchItemData.Identifier == absolutePathtoNewItem))
                return false;
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
            {
                parrent.SynchItemData.Size = size;
                parrent.SynchItemData.WriteDate = DateTime.Now;
            }
            return true;
        }

        public static bool UpdateExistingItem(SynchItem topSynchItem, string absolutePathtoNewItem, long size)
        {
            if (string.IsNullOrEmpty(absolutePathtoNewItem))
                return false;
            if (topSynchItem == null || topSynchItem.Items==null)
                return false;
            var parrent=GetItemLowestAvailableParrent(topSynchItem, absolutePathtoNewItem);
            if (parrent.Items[0].SynchItemData.Identifier == absolutePathtoNewItem)
                if (parrent.Items[0].SynchItemData.Size != size
                    || parrent.Items[0].SynchItemData.WriteDate.AddSeconds(30) < DateTime.Now)
                {
                    parrent.Items[0].SynchItemData.Size = size;
                    parrent.Items[0].SynchItemData.WriteDate = DateTime.Now;
                    return true;
                }
            return false;
        }

        public static bool DeleteItem(SynchItem topSynchItem, string absolutePathtoNewItem)
        {
            if (string.IsNullOrEmpty(absolutePathtoNewItem))
                return false;
            if (topSynchItem == null || topSynchItem.Items==null)
                return false;
            var parentItem = GetItemLowestAvailableParrent(topSynchItem, absolutePathtoNewItem);
            var currentItem = parentItem.Items.FirstOrDefault(i => i.SynchItemData.Identifier == absolutePathtoNewItem);
            if(currentItem!=null)
                parentItem.Items.Remove(currentItem);
            return true;
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
                if (currentItem == null ||currentItem.SynchItemData.Identifier==itemId)
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

    }
}
