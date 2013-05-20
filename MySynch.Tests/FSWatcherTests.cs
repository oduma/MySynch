using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            FSWatcher fsWatcher= new FSWatcher("",null,null,null);
        }
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FSWatcher_NoInsertQueue()
        {
            FSWatcher fsWatcher = new FSWatcher(@"c:\code\sciendo\mysynch\", null, null, null);
        }
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FSWatcher_NoUpdateQueue()
        {
            FSWatcher fsWatcher = new FSWatcher(@"c:\code\sciendo\mysynch\", queueOperation, null, null);
        }

        private void queueOperation(string obj)
        {
            return;
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FSWatcher_NoDeleteQueue()
        {
            FSWatcher fsWatcher = new FSWatcher(@"c:\code\sciendo\mysynch\", queueOperation, queueOperation, null);
        }
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void FSWatcher_FolderDoesNotExist()
        {
            FSWatcher fsWatcher = new FSWatcher(@"Data\me", queueOperation, queueOperation, queueOperation);
        }

    }
}
