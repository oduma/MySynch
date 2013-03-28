using System.ServiceModel;
using System.ServiceModel.Channels;

namespace MySynch.Common.WCF
{
    public static class ClientServerBindingHelper
    {
        public static Binding GetBinding(bool isDuplex)
        {
            if (isDuplex)
                return new WSDualHttpBinding();
            return new BasicHttpBinding();
        }
    }
}
