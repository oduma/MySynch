using System;
using System.ServiceModel;
using MySynch.Contracts;
using MySynch.Contracts.Messages;
using MySynch.Core.WCF.Clients;

namespace MySynch.Proxies
{
    public class SourceOfDataClient : BaseClient<ISourceOfData>, ISourceOfDataProxy
    {
        public RemoteResponse GetData(RemoteRequest remoteRequest)
        {
            RemoteResponse response = new RemoteResponse();
            try
            {
                using (new OperationContextScope((IContextChannel)Proxy))
                {
                    response = Proxy.GetData(remoteRequest);

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
    }
}
