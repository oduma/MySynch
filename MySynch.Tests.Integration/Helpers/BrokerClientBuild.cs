using System;
using System.ServiceModel;
using MySynch.Common.WCF.Clients;
using MySynch.Contracts;
using MySynch.Contracts.Messages;

namespace MySynch.Tests.Integration.Helpers
{
    public class BrokerClientBuild : BaseClient<IBroker>, IBrokerProxyBuild
    {
        #region IBrokerProxyBuild Members

        public GetHeartbeatResponse GetHeartbeat()
        {
            var response =
                new GetHeartbeatResponse();
            try
            {
                using (new OperationContextScope((IContextChannel) Proxy))
                {
                    response =
                        Proxy.GetHeartbeat();
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

        public AttachResponse Attach(AttachRequest request)
        {
            var response = new AttachResponse();
            try
            {
                using (new OperationContextScope((IContextChannel) Proxy))
                {
                    response =
                        Proxy.Attach(request);
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

        public DetachResponse Detach(DetachRequest request)
        {
            var response = new DetachResponse();
            try
            {
                using (new OperationContextScope((IContextChannel) Proxy))
                {
                    response =
                        Proxy.Detach(request);
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

        public ListAllRegistrationsResponse ListAllRegistrations()
        {
            var response =
                new ListAllRegistrationsResponse();
            try
            {
                using (new OperationContextScope((IContextChannel) Proxy))
                {
                    response =
                        Proxy.ListAllRegistrations();
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

        #endregion
    }

    public interface IBrokerProxyBuild : IBroker, IInitiateClient
    {
    }
}