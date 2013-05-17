using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MySynch.Contracts.Messages;
using MySynch.Core.Subscriber;
using MySynch.Tests.Stubs;
using NUnit.Framework;

namespace MySynch.Tests
{
    [TestFixture]
    public class CopyStrategyTests
    {
        [SetUp]
        public void Setup()
        {
            foreach (var oldBackupFile in Directory.GetFiles(@"Data\Output","cps*"))
            {
                if (!oldBackupFile.Contains("existingtarget.txt"))
                    File.Delete(oldBackupFile);
            }
            if(File.Exists(@"Data\Output\newtarget.txt"))
                File.Delete(@"Data\Output\newtarget.txt");
        }
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Copy_Nosource()
        {
            CopyStrategy copyStrategy=new CopyStrategy();
            copyStrategy.Initialize(new MockRemotePublisher());
            copyStrategy.Copy(null, "some target");
        }
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Copy_NoTarget()
        {
            CopyStrategy copyStrategy = new CopyStrategy();
            copyStrategy.Initialize(new MockRemotePublisher());
            copyStrategy.Copy("some source", "");
        }
        [Test]
        public void Copy_TargetExists_Ok()
        {
            CopyStrategy copyStrategy = new CopyStrategy();
            copyStrategy.Initialize(new MockRemotePublisher());
            Assert.True(copyStrategy.Copy("good source", @"Data\Output\existingtarget.txt"));
            CompareWithTheDataSource(@"Data\Output\existingtarget.txt");
            Assert.False(Directory.GetFiles(@"Data\Output", "cps*").Any());
        }

        private void CompareWithTheDataSource(string actualFile)
        {
            GetDataRequest request= new GetDataRequest();
            var expected = new MockRemotePublisher().GetData(request).Data;
            var actual = new byte[expected.Length];
            using (Stream fs = File.OpenRead(actualFile))
            {
                fs.Read(actual, 0, expected.Length);
            }
            for(int i=0;i<expected.Length;i++)
                Assert.AreEqual(expected[i],actual[i]);
            FileInfo fInfo = new FileInfo(actualFile);
            Assert.AreEqual(expected.Length,fInfo.Length);
        }

        [Test]
        public void Copy_TargetDoesNotExist_Ok()
        {
            CopyStrategy copyStrategy = new CopyStrategy();
            copyStrategy.Initialize(new MockRemotePublisher());
            Assert.True(copyStrategy.Copy("good source", @"Data\Output\newtarget.txt"));
            CompareWithTheDataSource(@"Data\Output\newtarget.txt");
            Assert.False(Directory.GetFiles(@"Data\Output", "cps*").Any());
        }
        [Test]
        public void Copy_TargetExists_Error()
        {
            CopyStrategy copyStrategy = new CopyStrategy();
            copyStrategy.Initialize(null);
            Assert.False(copyStrategy.Copy("good source", @"Data\Output\existingtarget.txt"));
            CompareWithFile(@"..\..\Data\Output\existingtarget.txt", @"Data\Output\existingtarget.txt");
            Assert.False(Directory.GetFiles(@"Data\Output", "cps*").Any());
        }

        private void CompareWithFile(string expectedFile, string actualFile)
        {
            Assert.True(File.Exists(actualFile));
            var expected = new FileInfo(expectedFile);
            var actual = new FileInfo(actualFile);
            Assert.AreEqual(expected.Length,actual.Length);
            var expectedContent = new byte[expected.Length];
            var actualContent = new byte[actual.Length];

            using (Stream fs = File.OpenRead(actualFile))
            {
                fs.Read(actualContent, 0, (int)actual.Length);
            }
            using (Stream fs = File.OpenRead(expectedFile))
            {
                fs.Read(expectedContent, 0, (int)expected.Length);
            }
            for (int i = 0; i < expected.Length; i++)
                Assert.AreEqual(expectedContent[i], actualContent[i]);
        }

        [Test]
        public void Copy_TargetDoesNotExist_Error()
        {
            CopyStrategy copyStrategy = new CopyStrategy();
            copyStrategy.Initialize(null);
            Assert.False(copyStrategy.Copy("good source", @"Data\Output\newtarget.txt"));
            Assert.False(File.Exists(@"Data\Output\newtarget.txt"));
            Assert.False(Directory.GetFiles(@"Data\Output", "cps*").Any());
        }


    }
}
