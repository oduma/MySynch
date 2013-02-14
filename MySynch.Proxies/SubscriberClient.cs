using System;
using System.ServiceModel;
using MySynch.Contracts;
using MySynch.Contracts.Messages;
using MySynch.Core.WCF.Clients;

namespace MySynch.Proxies
{
    public class SubscriberClient:BaseClient<ISubscriber>,ISubscriberProxy
    {
        public HeartbeatResponse GetHeartbeat()
        {
            HeartbeatResponse response = new HeartbeatResponse();
            try
            {
                using (new OperationContextScope((IContextChannel)Proxy))
                {
                    response = Proxy.GetHeartbeat();

                }
            }
            catch (CommunicationException e)
            {
                OnCommunicationException(e);
            }
            catch (TimeoutException e)
            {
                OnTimeoutException(e);
            }
            catch (Exception e)
            {
                OnException(e);
            }

            return response;

        }

        public bool ApplyChangePackage(ChangePushPackage changePushPackage, Func<string, string, bool> copyMethod)
        {
            bool response = false;
            try
            {
                using (new OperationContextScope((IContextChannel)Proxy))
                {
                    response = Proxy.ApplyChangePackage(changePushPackage,copyMethod);

                }
            }
            catch (CommunicationException e)
            {
                OnCommunicationException(e);
            }
            catch (TimeoutException e)
            {
                OnTimeoutException(e);
            }
            catch (Exception e)
            {
                OnException(e);
            }

            return response;

        }
    }
}
