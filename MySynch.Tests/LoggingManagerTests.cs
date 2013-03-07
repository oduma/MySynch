using System;
using System.IO;
using MySynch.Common;
using MySynch.Common.Logging;
using NUnit.Framework;

namespace MySynch.Tests
{
    [TestFixture]
    public class LoggingManagerTests
    {
        [SetUp]
        public void SetUp()
        {
            if (File.Exists("MySynchCoreDebug.log"))
            {
                var fs = File.Create("MySynchCoreDebug.log");
                fs.Flush();
                fs.Close();
            }

            if (File.Exists("MySynchCoreSystemError.log"))
            {
                var fs = File.Create("MySynchCoreSystemError.log");
                fs.Flush();
                fs.Close();
            }

            if (File.Exists("MySynchCorePerformance.log"))
            {
                var fs = File.Create("MySynchCorePerformance.log");
                fs.Flush();
                fs.Close();
            }
        }
        [Test]
        public void LogDebug_Ok()
        {
            var expectedText = "I wrote something in here";
            LoggingManager.Debug(expectedText);
            using (TextReader tr = File.OpenText("MySynchCoreDebug.log"))
            {
                var lineLogged = tr.ReadLine();
                Assert.True(lineLogged.Contains(expectedText));
            }
        }

        [Test]
        public void LogException_Ok()
        {
            var expectedText = "my excpetion";
            try
            {
                throw new Exception(expectedText);
            }
            catch (Exception ex)
            {
                LoggingManager.LogMySynchSystemError(ex);
                using (TextReader tr = File.OpenText("MySynchCoreSystemError.log"))
                {
                    var lineLogged = tr.ReadLine();
                    Assert.True(lineLogged.Contains(expectedText));
                }
            }
        }
        [Test]
        public void LogExceptionWithMessage_Ok()
        {
            var expectedText = "my excpetion";
            var expectedExplicitMessage = "my explicit message";
            try
            {
                throw new Exception(expectedText);
            }
            catch (Exception ex)
            {
                LoggingManager.LogMySynchSystemError(expectedExplicitMessage,ex);
                using (TextReader tr = File.OpenText("MySynchCoreSystemError.log"))
                {
                    var lineLogged = tr.ReadLine();
                    Assert.True(lineLogged.Contains(expectedText));
                    Assert.True(lineLogged.Contains(expectedExplicitMessage));
                }
            }
        }
        [Test]
        public void LogPerformnace_Ok()
        {
            using (LoggingManager.LogMySynchPerformance())
            {
                for (int i = 0; i < 1000; i++)
                {
                    ;
                }
            }
            using (TextReader tr = File.OpenText("MySynchCorePerformance.log"))
            {
                var lineLogged = tr.ReadLine();
                Assert.True(lineLogged.Contains("Started"));
                lineLogged = tr.ReadLine();
                Assert.True(lineLogged.Contains("Finished"));
            }

        }
        [Test]
        public void LogPerformanceWithInfo_Ok()
        {
            var expectedText = "my text";
            using (LoggingManager.LogMySynchPerformance(expectedText))
            {
                for (int i = 0; i < 1000; i++)
                {
                    ;
                }
            }
            using (TextReader tr = File.OpenText("MySynchCorePerformance.log"))
            {
                var lineLogged = tr.ReadLine();
                Assert.True(lineLogged.Contains("Started"));
                Assert.True(lineLogged.Contains(expectedText));
                lineLogged = tr.ReadLine();
                Assert.True(lineLogged.Contains("Finished"));
                Assert.True(lineLogged.Contains(expectedText));

            }

        }

    }
}
