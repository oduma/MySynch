using System.Collections.Generic;
using MySynch.Monitor.MVVM.ViewModels;

namespace MySynch.Monitor.Utils
{
    internal class MessageViewModelEqualityComparer:IEqualityComparer<MessageViewModel>
    {
        public bool Equals(MessageViewModel x, MessageViewModel y)
        {
            if (x == null || string.IsNullOrEmpty(x.FullTargetPath) || y == null || string.IsNullOrEmpty(y.FullTargetPath))
                return false;
            return (x.FullTargetPath == y.FullTargetPath);
        }

        public int GetHashCode(MessageViewModel obj)
        {
            return obj.GetHashCode();
        }
    }
}
