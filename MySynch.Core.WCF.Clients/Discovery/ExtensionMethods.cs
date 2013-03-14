using System.ServiceModel;

namespace MySynch.Core.WCF.Clients.Discovery
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
        public static BasicHttpBinding ApplyClientBinding(this BasicHttpBinding basicHttpBinding)
        {
            basicHttpBinding.MaxReceivedMessageSize = 650000000;
            basicHttpBinding.ReaderQuotas.MaxArrayLength = 650000000;
            return basicHttpBinding;
        }
    }
}
