﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MySynch.Core;
using NUnit.Framework;

namespace MySynch.Tests
{
    [TestFixture]
    public class SameSystemCopierTests
    {
        [Test]
        public void Copy_Ok()
        {
            Assert.False(File.Exists(@"Data\Output\Test\F1\F13\F13.xml"));
            Assert.True(SameSystemCopier.Copy(@"Data\Test\F1\F11\F13\F13.xml", @"Data\Output\Test\F1\F13\F13.xml"));
            Assert.True(File.Exists(@"Data\Output\Test\F1\F13\F13.xml"));
        }

        [Test]
        public void Copy_TargetParentFolder_Doesnt_Exist()
        {
            Assert.False(File.Exists(@"Data\Output\Test\F1\F11\F13\F13.xml"));
            Assert.True(SameSystemCopier.Copy(@"Data\Test\F1\F11\F13\F13.xml", @"Data\Output\Test\F1\F11\F13\F13.xml"));
            Assert.True(File.Exists(@"Data\Output\Test\F1\F11\F13\F13.xml"));
        }

        [Test]
        public void Copy_Source_Does_Not_Exist()
        {
            Assert.False(SameSystemCopier.Copy(@"Data\Test\F2\F11\F13\F13.xml", @"Data\Output\Test\F1\F11\F13\F13.xml"));
        }

        [Test]
        public void Copy_TargetExists()
        {
            Assert.True(File.Exists(@"Data\Output\Test\F1\F12\F12.xml"));
            FileInfo fInfo = new FileInfo(@"Data\Output\Test\F1\F12\F12.xml");
            var initial = fInfo.Length;
            Assert.True(SameSystemCopier.Copy(@"Data\Test\F1\F11\F12\F12.xml", @"Data\Output\Test\F1\F12\F12.xml"));
            Assert.True(File.Exists(@"Data\Output\Test\F1\F12\F12.xml"));
            fInfo = new FileInfo(@"Data\Output\Test\F1\F12\F12.xml");
            var final = fInfo.Length;
            Assert.Greater(final,initial);

        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Copy_Source_NotSend()
        {
            SameSystemCopier.Copy(null, @"Data\Output\Test\F1\F12\F12.xml");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Copy_Target_NotSend()
        {
            SameSystemCopier.Copy(@"Data\Test\F1\F11\F12\F12.xml", string.Empty);
        }
    }
}