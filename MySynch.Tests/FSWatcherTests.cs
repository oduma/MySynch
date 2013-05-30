using System;
using MySynch.Contracts.Messages;
using MySynch.Core.Publisher;
using NUnit.Framework;

namespace MySynch.Tests
{
    [TestFixture]
    public class FSWatcherTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FSWatcher_NoRootFolder()
        {
            FSWatcher fsWatcher= new FSWatcher("",null,null);
        }
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FSWatcher_NoQueueProcessor()
        {
            FSWatcher fsWatcher = new FSWatcher(@"c:\code\sciendo\mysynch\", null,null);
        }
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FSWatcher_NoRenameQueueProcessor()
        {
            FSWatcher fsWatcher = new FSWatcher(@"c:\code\sciendo\mysynch\", queueOperation, null);
        }

        private void queueOperation(string obj, OperationType operationType)
        {
            return;
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void FSWatcher_FolderDoesNotExist()
        {
            FSWatcher fsWatcher = new FSWatcher(@"Data\me", queueOperation,renameQueueOperation);
        }

        private void renameQueueOperation(string arg1, string arg2)
        {
            return;
        }
    }
}
