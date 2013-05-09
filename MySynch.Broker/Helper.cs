using System.Configuration;
using System.Linq;
using MySynch.Common.Serialization;

namespace MySynch.Broker
{
    public static class Helper
    {
        public static StoreType ReadTheNodeConfiguration()
        {
            var key = ConfigurationManager.AppSettings.AllKeys.FirstOrDefault(k => k == "StoreName");
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


    }
}
