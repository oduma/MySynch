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
            return ListItems(_items, parentItemId);
        }

        private List<SynchItem> ListItems(List<SynchItem> list, string parentItemId)
        {
            SynchItem parentItem = list.FirstOrDefault(l => l.Identifier == parentItemId);
            if (parentItem != null)
                return parentItem.Items;
            foreach (SynchItem si in list)
            {
                List<SynchItem> sItems=ListItems(si.Items,parentItemId);
                if (sItems != null)
                    return sItems;
                else
                    return null;
            }
            return null;
        }

        public int InsertItems(string parentItemId, List<SynchItem> Items)
        {
            throw new NotImplementedException();
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
