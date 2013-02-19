using System;
using System.ServiceModel;
using MySynch.Contracts;
using MySynch.Contracts.Messages;
using MySynch.Core.WCF.Clients;

namespace MySynch.Proxies
{
    public class DistributorMonitorClient:BaseClient<IDistributorMonitor>, IDistributorMonitorProxy
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

        public DistributorComponent ListAvailableComponentsTree()
        {
            DistributorComponent response = new DistributorComponent();
            try
            {
                using (new OperationContextScope((IContextChannel)Proxy))
                {
                    response = Proxy.ListAvailableComponentsTree();

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

        public void ReEvaluateAllChannels()
        {
            try
            {
                using (new OperationContextScope((IContextChannel)Proxy))
                {
                    Proxy.ReEvaluateAllChannels();

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

        }
    }
}
