using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            SynchItem parentItem = GetParentItem(parentItemId);
            if (parentItem == null)
                return null;
            return (parentItem.Items)??new List<SynchItem>();
        }

        private SynchItem GetParentItem(string parentItemId)
        {
            string[] levels = parentItemId.Split(new char[] { '\\' });

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
            SynchItem parentItem = GetParentItem(parentItemId);
            if (parentItem.Items.FirstOrDefault(i=>Items.Contains(i,new SynchItemEqualityComparer()))!=null)
            {
                parentItem.Items.AddRange(Items);
                return Items.Count;
            }
            return 0;
        }

        public void UpdateItem(string Identifier, string Name)
        {
            throw new NotImplementedException();
        }

        public bool RemoveItem(string Identifier)
        {
            throw new NotImplementedException();
        }

        public int RemoveItems(string parentItemId)
        {
            throw new NotImplementedException();
        }
    }
}
