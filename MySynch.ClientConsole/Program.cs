using System;
using MySynch.Core;
using MySynch.Core.DataTypes;
using MySynch.Core.Interfaces;

namespace MySynch.ClientConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Monitoring... (close this window to stop it)");
            //Start monitoring the queue and respond to a number of situations
            string sourceRootFolder=@"C:\Code\Sciendo\MySynch\MySynch.ClientConsole\bin\Debug";
            IChangePublisher changePublisher = new ChangePublisher();
            changePublisher.Initialize(sourceRootFolder);

            FSWatcher fsWatcher= new FSWatcher(changePublisher);
            changePublisher.ItemsQueued += fsWatcher_ItemsQueued;
            Console.WriteLine("Monitoring path: " + fsWatcher.Path);
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        }

        static void fsWatcher_ItemsQueued(object sender, ItemsQueuedEventArgs e)
        {
            foreach(var key in e.NonPublishedItems.Keys)
                Console.WriteLine("{0} recorded on path {1}",e.NonPublishedItems[key],key);
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
