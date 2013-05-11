using System.Configuration;
using System.Linq;
using MySynch.Common.Serialization;
using MySynch.Core.DataTypes;

namespace MySynch.Core
{
    public static class ConfigurationHelper
    {
        public static StoreType ReadBrokerNodeConfiguration()
        {
            var key = Enumerable.FirstOrDefault<string>(ConfigurationManager.AppSettings.AllKeys, k => k == "StoreName");
            string storeName;
            if (key == null)
                storeName = "brokerstoredata.xml";
            else
                storeName = ConfigurationManager.AppSettings[key];
            key = ConfigurationManager.AppSettings.AllKeys.FirstOrDefault(k => k == "StoreType");
            string storeTypeName;
            if (key == null)
                storeTypeName = "IStore.Registration.FileSystemStore";
            else
                storeTypeName = ConfigurationManager.AppSettings[key];
            return new StoreType { StoreName = storeName, StoreTypeName = storeTypeName };
        }

        public static PublisherConfig ReadPublisherNodeConfiguration()
        {
            var key = Enumerable.FirstOrDefault<string>(ConfigurationManager.AppSettings.AllKeys, k => k == "BrokerName");
            string brokerName;
            if (key == null)
                brokerName = "localhost";
            else
                brokerName = ConfigurationManager.AppSettings[key];
            key = Enumerable.FirstOrDefault<string>(ConfigurationManager.AppSettings.AllKeys, k => k == "LocalRootFolder");
            string rootFolder;
            if (key == null)
                rootFolder = string.Empty;
            else
                rootFolder = ConfigurationManager.AppSettings[key];

            return new PublisherConfig {BrokerName = brokerName, RootFolder = rootFolder};

        }

    }
}
