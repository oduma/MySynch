using System.Configuration;

namespace MySynch.Core.Configuration
{
    public class MySynchConfigurationSection:ConfigurationSection
    {
        [ConfigurationProperty("CommunicationLogging", DefaultValue = "notverbose", IsRequired = false)]
        public string CommunicationLogging
        {
            get
            {
                return (string)this["CommunicationLogging"];
            }
            set
            {
                this["CommunicationLogging"] = value;
            }
        }

        [ConfigurationProperty("InstanceName", DefaultValue = "", IsRequired = true)]
        public string InstanceName
        {
            get
            {
                return (string)this["InstanceName"];
            }
            set
            {
                this["InstanceName"] = value;
            }
        }
    }
}
