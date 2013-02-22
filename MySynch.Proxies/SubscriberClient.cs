using System;
using System.Collections.Generic;
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

        public bool ApplyChangePackage(ChangePushPackage changePushPackage)
        {
            bool response = false;
            try
            {
                using (new OperationContextScope((IContextChannel)Proxy))
                {
                    response = Proxy.ApplyChangePackage(changePushPackage);

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

        public string GetTargetRootFolder()
        {
            string response=string.Empty;
            try
            {
                using (new OperationContextScope((IContextChannel)Proxy))
                {
                    response = Proxy.GetTargetRootFolder();

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

        public bool TryOpenChannel(string sourceOfDataEndpointName)
        {
            bool response = false;
            try
            {
                using (new OperationContextScope((IContextChannel)Proxy))
                {
                    response = Proxy.TryOpenChannel(sourceOfDataEndpointName);

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
