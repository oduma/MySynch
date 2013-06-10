using System.Configuration;
using MySynch.Core.Configuration;
using MySynch.Core.DataTypes;
using NUnit.Framework;

namespace MySynch.Tests
{
    [TestFixture]
    public class ConfigurationTests
    {
        [Test]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void BrokerConfigurationWithDefaultValues_Ok()
        {
            var configSection =
                (MySynchBrokerConfigurationSection)
                ConfigurationManager.GetSection("MySynchBrokerEmpty"); 
        }

        [Test]
        public void BrokerConfigurationWithValues_MinimalValues_Ok()
        {
            var configSection =
                (MySynchBrokerConfigurationSection)
                ConfigurationManager.GetSection("MySynchBrokerMinimal");

            Assert.IsNotNull(configSection);
            Assert.AreEqual(configSection.StoreName, "brokerstoredata.xml");
            Assert.AreEqual(configSection.StoreType, "IStore.Registration.FileSystemStore");
            Assert.AreEqual(configSection.CommunicationLogging, "notverbose");
            Assert.AreEqual(configSection.InstanceName, "mybroker");
            Assert.AreEqual(configSection.MonitorInstanceName, "mybrokermonitor");
        }

        [Test]
        public void BrokerCofnigurationWithAllValues_Ok()
        {
            var configSection =
    (MySynchBrokerConfigurationSection)
    ConfigurationManager.GetSection("MySynchBrokerAll");

            Assert.IsNotNull(configSection);
            Assert.AreEqual(configSection.StoreName, "mystorename");
            Assert.AreEqual(configSection.StoreType, "mystoretype");
            Assert.AreEqual(configSection.CommunicationLogging, "verbose");
            Assert.AreEqual(configSection.InstanceName, "mybroker");
            Assert.AreEqual(configSection.MonitorInstanceName, "mybrokermonitor");

        }
        [Test]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void LCConfigurationWithDefaultValues_Ok()
        {
            var configSection =
                (MySynchLocalComponentConfigurationSection)
                ConfigurationManager.GetSection("MySynchLCEmpty");
        }

        [Test]
        public void LCConfigurationWithValues_MinimalValues_Ok()
        {
            var configSection =
                (MySynchLocalComponentConfigurationSection)
                ConfigurationManager.GetSection("MySynchLCMinimal");


            Assert.IsNotNull(configSection);
            Assert.AreEqual(configSection.LocalRootFolder, "mylocalRootFolder");
            Assert.AreEqual(configSection.BrokerUrl, "myBrokerUrl");
            Assert.AreEqual(configSection.CommunicationLogging, "notverbose");
            Assert.AreEqual(configSection.InstanceName, "myLC");
            Assert.AreEqual(configSection.MonitorInstanceName,"myLCmonitor");
        }

        [Test]
        public void LCCofnigurationWithAllValues_Ok()
        {
            var configSection =
    (MySynchLocalComponentConfigurationSection)
    ConfigurationManager.GetSection("MySynchLCAll");

            Assert.IsNotNull(configSection);
            Assert.AreEqual(configSection.LocalRootFolder, "mylocalRootFolder");
            Assert.AreEqual(configSection.BrokerUrl, "myBrokerUrl");
            Assert.AreEqual(configSection.CommunicationLogging, "verbose");
            Assert.AreEqual(configSection.InstanceName, "myLC");
            Assert.AreEqual(configSection.MonitorInstanceName,"myLCmonitor");
        }
    }
}
