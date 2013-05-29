using System.Configuration;

namespace MySynch.Core.Configuration
{
    public class MySynchPublisherConfigurationSection:MySynchLocalComponentConfigurationSection
    {
        [ConfigurationProperty("ConsiderOfflineChanges", DefaultValue = "No", IsRequired = false)]
        public string ConsiderOfflineChanges
        {
            get
            {
                return (string)this["ConsiderOfflineChanges"];
            }
            set
            {
                this["ConsiderOfflineChanges"] = value;
            }
        }
    }
}
