using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace MySynch.Common.WCF
{
    public static class ClientServerBindingHelper
    {
        public static Binding GetBinding(bool isDuplex) 
        {
            if (isDuplex)
            {
                WSDualHttpBinding wsDualHttpBinding=new WSDualHttpBinding();
                wsDualHttpBinding.SendTimeout = TimeSpan.FromMinutes(25);
                return wsDualHttpBinding;
            }
            BasicHttpBinding basicHttpBinding= new BasicHttpBinding();
            basicHttpBinding.SendTimeout = TimeSpan.FromMinutes(25);
            return basicHttpBinding;
        }
    }
}
