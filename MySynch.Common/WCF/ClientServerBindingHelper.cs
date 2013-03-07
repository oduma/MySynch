using System.ServiceModel;

namespace MySynch.Common.WCF
{
    public static class ClientServerBindingHelper
    {
        public static BasicHttpBinding GetBinding()
        {
            return new BasicHttpBinding();
        }
    }
}
