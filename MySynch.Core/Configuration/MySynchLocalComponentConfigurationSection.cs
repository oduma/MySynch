using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace MySynch.Core.Configuration
{
    public class MySynchLocalComponentConfigurationSection:MySynchConfigurationSection
    {
        [ConfigurationProperty("LocalRootFolder", DefaultValue = "", IsRequired = true)]
        public string LocalRootFolder
        {
            get
            {
                return (string)this["LocalRootFolder"];
            }
            set
            {
                this["LocalRootFolder"] = value;
            }
        }

        [ConfigurationProperty("BrokerUrl", DefaultValue = "", IsRequired = true)]
        public string BrokerUrl
        {
            get
            {
                return (string)this["BrokerUrl"];
            }
            set
            {
                this["BrokerUrl"] = value;
            }
        }

    }
}
