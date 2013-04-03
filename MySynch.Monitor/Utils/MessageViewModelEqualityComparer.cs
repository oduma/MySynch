using System.Collections.Generic;
using MySynch.Monitor.MVVM.ViewModels;

namespace MySynch.Monitor.Utils
{
    internal class MessageViewModelEqualityComparer:IEqualityComparer<MessageViewModel>
    {
        public bool Equals(MessageViewModel x, MessageViewModel y)
        {
            if (x == null || string.IsNullOrEmpty(x.RelativePath) || y == null || string.IsNullOrEmpty(y.RelativePath))
                return false;
            return (x.RelativePath == y.RelativePath);
        }

        public int GetHashCode(MessageViewModel obj)
        {
            return obj.GetHashCode();
        }
    }
}
