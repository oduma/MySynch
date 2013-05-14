using System;
using System.IO;
using System.Threading;
using MySynch.Common.Logging;

namespace MySynch.Core.Publisher
{
    public class FSWatcher
    {
        public string Path { get; private set; }

        private Action<string> _queueInsert;
        private Action<string> _queueUpdate;
        private Action<string> _queueDelete;

        public FSWatcher(string localRootFolder,Action<string> queueInsert, Action<string> queueUpdate, Action<string> queueDelete)
        {
            LoggingManager.Debug("Initializing the FS Watcher with publisher: " +localRootFolder);
            if(string.IsNullOrEmpty(localRootFolder))
                throw new ArgumentNullException("localRootFolder");
            _queueInsert = queueInsert;
            _queueUpdate = queueUpdate;
            _queueDelete = queueDelete;

            FileSystemWatcher fsWatcher = new FileSystemWatcher(localRootFolder);
            fsWatcher.IncludeSubdirectories = true;
            

            Path = fsWatcher.Path;

            fsWatcher.Created += fsWatcher_Created;
            fsWatcher.Changed += fsWatcher_Changed;
            fsWatcher.Deleted += fsWatcher_Deleted;
            fsWatcher.Renamed += fsWatcher_Renamed;
            fsWatcher.EnableRaisingEvents = true;
            LoggingManager.Debug("Initilization done waiting for changes in the FS.");
        }


        private void fsWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            LoggingManager.Debug("A File renamed from " + e.OldFullPath + " to " + e.FullPath);
            //if it is a directory ignore it
            if (!File.Exists(e.FullPath))
                return;
            //queue an insert;
            // Wait if file is still open
            FileInfo fileInfo = new FileInfo(e.FullPath);
            while (IsFileLocked(fileInfo))
            {
                Thread.Sleep(500);
            }

            //queue a delete
            //queue an insert
            _queueDelete(e.OldFullPath);
            _queueInsert(e.FullPath);

        }

        private void fsWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            LoggingManager.Debug("A file deleted: " + e.FullPath);
            if (Directory.Exists(e.FullPath))
                return;
            _queueDelete(e.FullPath);
        }

        private void fsWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            LoggingManager.Debug("A file changed: " + e.FullPath);
            //if it is a directory ignore it
            if (!File.Exists(e.FullPath))
                return;
            //queue an insert;
            // Wait if file is still open
            FileInfo fileInfo = new FileInfo(e.FullPath);
            while (IsFileLocked(fileInfo))
            {
                Thread.Sleep(500);
            }
            //queue an update;
            _queueUpdate(e.FullPath);
        }

        private void fsWatcher_Created(object sender, FileSystemEventArgs e)
        {
            LoggingManager.Debug("A new file created: " + e.FullPath);
            //if it is a directory ignore it
            if (!File.Exists(e.FullPath))
                return;
            //queue an insert;
            // Wait if file is still open
            FileInfo fileInfo = new FileInfo(e.FullPath);
            while (IsFileLocked(fileInfo))
            {
                Thread.Sleep(500);
            }
            _queueInsert(e.FullPath);
        }

        static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open,
                         FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

    }
}
