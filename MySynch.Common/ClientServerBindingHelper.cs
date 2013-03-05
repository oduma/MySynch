using System.ServiceModel;

namespace MySynch.Common
{
    public static class ClientServerBindingHelper
    {
        public static BasicHttpBinding GetBinding()
        {
            return new BasicHttpBinding();
        }
    }
}
