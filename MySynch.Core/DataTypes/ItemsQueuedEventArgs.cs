using System;
using System.Collections.Generic;
using MySynch.Contracts.Messages;

namespace MySynch.Core.DataTypes
{
    public class ItemsQueuedEventArgs:EventArgs
    {
        public SortedList<string, OperationType> NonPublishedItems { get; private set; }

        public ItemsQueuedEventArgs(SortedList<string,OperationType> items)
        {
            NonPublishedItems = items;
        }
    }
}
