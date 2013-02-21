using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySynch.Contracts.Messages;
using MySynch.Core.DataTypes;

namespace MySynch.Core.Interfaces
{
    public interface ISynchItemManager
    {
        List<SynchItem> ListAllItems();

        List<SynchItem> ListItems(string parentItemId);

        int InsertItems(string parentItemId, List<SynchItem> Items);

        void UpdateItem(string Identifier, string Name);

        bool RemoveItem(string Identifier);

        int RemoveItems(string parentItemId);
    }
}
