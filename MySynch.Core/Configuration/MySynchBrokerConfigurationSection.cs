using System.Configuration;

namespace MySynch.Core.Configuration
{
    public class MySynchBrokerConfigurationSection:MySynchConfigurationSection
    {
        [ConfigurationProperty("StoreName", DefaultValue = "brokerstoredata.xml", IsRequired = false)]
        public string StoreName
        {
            get
            {
                return (string)this["StoreName"];
            }
            set
            {
                this["StoreName"] = value;
            }
        }

        [ConfigurationProperty("StoreType", DefaultValue = "IStore.Registration.FileSystemStore", IsRequired = false)]
        public string StoreType
        {
            get
            {
                return (string)this["StoreType"];
            }
            set
            {
                this["StoreType"] = value;
            }
        }
    }
}
