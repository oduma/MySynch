﻿using System;
using System.IO;
using MySynch.Core;
using MySynch.Core.Subscriber;
using NUnit.Framework;

namespace MySynch.Tests
{
    [TestFixture]
    public class SameSystemCopierTests
    {
        [Test]
        public void Copy_Ok()
        {
            if (File.Exists(@"Data\Output\Test\F1\F13\F13.xml"))
                File.Delete(@"Data\Output\Test\F1\F13\F13.xml");
            Assert.False(File.Exists(@"Data\Output\Test\F1\F13\F13.xml"));
            CopyStrategy sameSystemCopier = new CopyStrategy();
            sameSystemCopier.Initialize(null);
            Assert.True(sameSystemCopier.Copy(@"Data\Test\F1\F11\F13\F13.xml", @"Data\Output\Test\F1\F13\F13.xml"));
            Assert.True(File.Exists(@"Data\Output\Test\F1\F13\F13.xml"));
        }

        [Test]
        public void Copy_TargetParentFolder_Doesnt_Exist()
        {
            if (File.Exists(@"Data\Output\Test\F1\F11\F13\F13.xml"))
                File.Delete(@"Data\Output\Test\F1\F11\F13\F13.xml");
            Assert.False(File.Exists(@"Data\Output\Test\F1\F11\F13\F13.xml"));
            Assert.True(new CopyStrategy().Copy(@"Data\Test\F1\F11\F13\F13.xml", @"Data\Output\Test\F1\F11\F13\F13.xml"));
            Assert.True(File.Exists(@"Data\Output\Test\F1\F11\F13\F13.xml"));
        }

        [Test]
        public void Copy_Source_Does_Not_Exist()
        {
            Assert.False(new CopyStrategy().Copy(@"Data\Test\F2\F11\F13\F13.xml", @"Data\Output\Test\F1\F11\F13\F13.xml"));
        }

        [Test]
        public void Copy_TargetExists()
        {
            Assert.True(File.Exists(@"Data\Output\Test\F1\F12\F123.xml"));
            FileInfo fInfo = new FileInfo(@"Data\Output\Test\F1\F12\F123.xml");
            var initial = fInfo.Length;
            Assert.True(new CopyStrategy().Copy(@"Data\Test\F1\F11\F12\F12.xml", @"Data\Output\Test\F1\F12\F123.xml"));
            Assert.True(File.Exists(@"Data\Output\Test\F1\F12\F123.xml"));
            fInfo = new FileInfo(@"Data\Output\Test\F1\F12\F123.xml");
            var final = fInfo.Length;
            Assert.Greater(final,initial);

        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Copy_Source_NotSend()
        {
            new CopyStrategy().Copy(null, @"Data\Output\Test\F1\F12\F12.xml");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Copy_Target_NotSend()
        {
            new CopyStrategy().Copy(@"Data\Test\F1\F11\F12\F12.xml", string.Empty);
        }
    }
}
