//This code was auto generated with a tool
//do not change this file
using System;
using System.ServiceModel;
using MySynch.Contracts;
using MySynch.Contracts.Messages;
using MySynch.Core.WCF.Clients.Discovery;
using MySynch.Proxies.Interfaces;

namespace MySynch.Proxies
{
	public class SubscriberClient :BaseClient<ISubscriber>,ISubscriberProxy
	{
		public MySynch.Contracts.Messages.GetHeartbeatResponse GetHeartbeat() 
		{
		MySynch.Contracts.Messages.GetHeartbeatResponse response = new MySynch.Contracts.Messages.GetHeartbeatResponse(); 
		try
		{
		                using (new OperationContextScope((IContextChannel)Proxy))
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

			return  response; 

		}
		public void ConsumePackage(MySynch.Contracts.Messages.PublishPackageRequestResponse publishPackageRequestResponse) 
		{
		 
		try
		{
		                using (new OperationContextScope((IContextChannel)Proxy))
                {
				 
					Proxy.ConsumePackage(publishPackageRequestResponse);
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

			return ; 

		}
		public MySynch.Contracts.Messages.TryOpenChannelResponse TryOpenChannel(MySynch.Contracts.Messages.TryOpenChannelRequest sourceOfDataEndpointName) 
		{
		MySynch.Contracts.Messages.TryOpenChannelResponse response = new MySynch.Contracts.Messages.TryOpenChannelResponse(); 
		try
		{
		                using (new OperationContextScope((IContextChannel)Proxy))
                {
				 response =  
					Proxy.TryOpenChannel(sourceOfDataEndpointName);
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

			return  response; 

		}
		public void ForceSetTheSubscriberFeedback(MySynch.Contracts.ISubscriberFeedback SubscriberFeedback) 
		{
		 
		try
		{
		                using (new OperationContextScope((IContextChannel)Proxy))
                {
				 
					Proxy.ForceSetTheSubscriberFeedback(SubscriberFeedback);
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

			return ; 

		}
	}
}
		