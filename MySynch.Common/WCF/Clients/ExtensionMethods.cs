using System.ServiceModel;
using System.ServiceModel.Channels;

namespace MySynch.Common.WCF.Clients
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// This is the equivalent of applying the following extra 
        /// binding properties:
        ///  maxReceivedMessageSize="65000000"
        ///readerQuotas maxArrayLength="650000000"
        /// </summary>
        /// <param name="basicHttpBinding"></param>
        /// <returns></returns>
        public static BasicHttpBinding ApplyClientBasicHttpBinding(this Binding basicHttpBinding)
        {
            ((BasicHttpBinding)basicHttpBinding).MaxReceivedMessageSize = 650000000;
            ((BasicHttpBinding)basicHttpBinding).ReaderQuotas.MaxArrayLength = 650000000;
            return ((BasicHttpBinding)basicHttpBinding);
        }

        public static WSDualHttpBinding ApplyClientWsDualHttpBinding(this Binding wsDualHttpBinding)
        {
            ((WSDualHttpBinding)wsDualHttpBinding).MaxReceivedMessageSize = 650000000;
            ((WSDualHttpBinding)wsDualHttpBinding).ReaderQuotas.MaxArrayLength = 650000000;
            return ((WSDualHttpBinding)wsDualHttpBinding);
        }

        
    }
}
