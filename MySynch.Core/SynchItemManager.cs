using System;
using System.Collections.Generic;
using System.Linq;
using MySynch.Contracts.Messages;
using MySynch.Core.Interfaces;
using MySynch.Core.DataTypes;

namespace MySynch.Core
{
    public class SynchItemManager : ISynchItemManager
    {
        private List<SynchItem> _items;

        public SynchItemManager()
        {
            _items = new List<SynchItem>();
        }

        public SynchItemManager(List<SynchItem> initialLoad)
        {
            _items = initialLoad;
        }

        public List<SynchItem> ListAllItems()
        {
            if (_items == null)
            {
                _items = new List<SynchItem>();
            }
            return _items;
        }

        public List<SynchItem> ListItems(string parentItemId)
        {
            if (string.IsNullOrEmpty(parentItemId))
                throw new ArgumentNullException("parentItemId");
            if (_items == null)
                return new List<SynchItem>();

            SynchItem parentItem = GetItem(parentItemId);
            if (parentItem == null)
                return null;
            return (parentItem.Items)??new List<SynchItem>();
        }

        private SynchItem GetItem(string itemId)
        {
            string[] levels = itemId.Split(new char[] { '\\' });

            SynchItem parentItem = null;
            string currentLevel = string.Empty;
            List<SynchItem> list = _items;
            foreach (string level in levels)
            {
                if (list == null)
                    return null;
                currentLevel = (string.IsNullOrEmpty(currentLevel)) ? level : string.Format("{0}\\{1}", currentLevel, level);
                parentItem = list.FirstOrDefault(i => i.Identifier == currentLevel);
                if (parentItem == null)
                    return null;
                list = parentItem.Items;
            }
            return parentItem;
        }

        public int InsertItems(string parentItemId, List<SynchItem> Items)
        {
            if (string.IsNullOrEmpty(parentItemId))
                throw new ArgumentNullException("parentItemId");
            if (_items == null)
                return 0;
            if (Items == null || Items.Count == 0)
                return 0;
            SynchItem parentItem = GetItem(parentItemId);
            if (parentItem == null)
                throw new ArgumentException("Item not found", parentItemId);

            if (parentItem.Items == null)
            {
                parentItem.Items = Items;
                return Items.Count;
            }
            if (parentItem.Items.FirstOrDefault(i=>Items.Contains(i,new SynchItemEqualityComparer()))==null)
            {
                parentItem.Items.AddRange(Items);
                return Items.Count;
            }
            return 0;
        }

        public void UpdateItem(string Identifier, string Name)
        {
            if (string.IsNullOrEmpty(Identifier))
                throw new ArgumentNullException("Identifier");
            if (string.IsNullOrEmpty(Name))
                throw new ArgumentNullException("Name");
            if (_items == null)
                return;
            var item = GetItem(Identifier);
            if (item == null)
                throw new ArgumentException("Item not found", "Identifier");
            item.Name = Name;
        }

        public bool RemoveItem(string Identifier)
        {
            if (string.IsNullOrEmpty(Identifier))
                throw new ArgumentNullException("Identifier");
            if (_items == null)
                return false;
            var item=GetItem(Identifier);
            if (item == null)
                throw new ArgumentException("Item not found", "Identifier");
            var parentItem = GetParentItem(Identifier);
            if (parentItem == null)
            {
                _items = new List<SynchItem>();
                return true;
            }
            parentItem.Items.Remove(item);
            return true;
        }

        private SynchItem GetParentItem(string Identifier)
        {
            string[] levels = Identifier.Split(new char[] { '\\' });
            if (levels.Count() <= 1)
                return null;
            string parentIdentifier=string.Empty;
            for(int i=0;i<levels.Count()-1;i++)
                parentIdentifier= (string.IsNullOrEmpty(parentIdentifier)) ? levels[i] : string.Format("{0}\\{1}", parentIdentifier, levels[i]);

            return GetItem(parentIdentifier);
        }

        public int RemoveItems(string parentItemId)
        {
            if (string.IsNullOrEmpty(parentItemId))
                throw new ArgumentNullException("parentItemId");
            if (_items == null)
                return 0;
            var item = GetItem(parentItemId);
            if (item == null)
                throw new ArgumentException("Item not found", "parentItemId");
            var itemsCount = item.Items.Count;
            item.Items.Clear();
            return itemsCount;
        }
    }
}
