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
		public MySynch.Contracts.Messages.ApplyChangePackageResponse ApplyChangePackage(MySynch.Contracts.Messages.PublishPackageRequestResponse publishPackageRequestResponse) 
		{
		MySynch.Contracts.Messages.ApplyChangePackageResponse response = new MySynch.Contracts.Messages.ApplyChangePackageResponse(); 
		try
		{
		                using (new OperationContextScope((IContextChannel)Proxy))
                {
				 response =  
					Proxy.ApplyChangePackage(publishPackageRequestResponse);
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
		public MySynch.Contracts.Messages.GetTargetFolderResponse GetTargetRootFolder() 
		{
		MySynch.Contracts.Messages.GetTargetFolderResponse response = new MySynch.Contracts.Messages.GetTargetFolderResponse(); 
		try
		{
		                using (new OperationContextScope((IContextChannel)Proxy))
                {
				 response =  
					Proxy.GetTargetRootFolder();
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
	}
}
		