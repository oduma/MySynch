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
		public MySynch.Contracts.Messages.GetDataResponse GetData(MySynch.Contracts.Messages.GetDataRequest request) 
		{
		MySynch.Contracts.Messages.GetDataResponse response = new MySynch.Contracts.Messages.GetDataResponse(); 
		try
		{
		                using (new OperationContextScope((IContextChannel)Proxy))
                {
				 response =  
					Proxy.GetData(request);
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
		