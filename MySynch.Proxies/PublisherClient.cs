//This code was auto generated with a tool
//do not change this file
using System;
using System.ServiceModel;
using MySynch.Contracts;
using MySynch.Contracts.Messages;
using MySynch.Core.WCF.Clients;
using MySynch.Proxies.Interfaces;

namespace MySynch.Proxies
{
	public class PublisherClient :BaseClient<IPublisher>,IPublisherProxy
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
		public MySynch.Contracts.Messages.PublishPackageRequestResponse PublishPackage() 
		{
		MySynch.Contracts.Messages.PublishPackageRequestResponse response = new MySynch.Contracts.Messages.PublishPackageRequestResponse(); 
		try
		{
		                using (new OperationContextScope((IContextChannel)Proxy))
                {
				 response =  
					Proxy.PublishPackage();
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
		public void RemovePackage(MySynch.Contracts.Messages.PublishPackageRequestResponse packageRequestResponsePublished) 
		{
		 
		try
		{
		                using (new OperationContextScope((IContextChannel)Proxy))
                {
				 
					Proxy.RemovePackage(packageRequestResponsePublished);
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
		