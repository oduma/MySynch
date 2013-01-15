using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MySynch.ClientConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Monitoring... (press enter to stop it)");
            //Start monitoring the queue and respond to a number of situations
            string sourceRootFolder;
            FileSystemWatcher fsWatcher= new FileSystemWatcher(sourceRootFolder);
            fsWatcher.Created += new FileSystemEventHandler(fsWatcher_Created);
            fsWatcher.Changed += new FileSystemEventHandler(fsWatcher_Changed);
            fsWatcher.Deleted += new FileSystemEventHandler(fsWatcher_Deleted);
            fsWatcher.Renamed += new RenamedEventHandler(fsWatcher_Renamed);
            
            do
            {
                
            } while (Console.ReadLine()!=string.Empty);

        }

        static void fsWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            //queue a delete
            //queue an insert
        }

        static void fsWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            //queue a delete
        }

        static void fsWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            //queue an update;
        }

        static void fsWatcher_Created(object sender, FileSystemEventArgs e)
        {
            //queue an insert;
        }

    }
}
