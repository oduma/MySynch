using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MySynch.Core;

namespace MySynch.ClientConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Monitoring... (close this window to stop it)");
            //Start monitoring the queue and respond to a number of situations
            string sourceRootFolder=@"C:\Code\Sciendo\MySynch\MySynch.ClientConsole\bin\Debug";
            FSWatcher fsWatcher= new FSWatcher(sourceRootFolder,Environment.MachineName);
            Console.WriteLine("Monitoring path: " + fsWatcher.Path);
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        }
    }
}
