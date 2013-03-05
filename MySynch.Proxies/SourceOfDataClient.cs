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
	public class SourceOfDataClient :BaseClient<ISourceOfData>,ISourceOfDataProxy
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
		public MySynch.Contracts.Messages.RemoteResponse GetData(MySynch.Contracts.Messages.RemoteRequest remoteRequest) 
		{
		MySynch.Contracts.Messages.RemoteResponse response = new MySynch.Contracts.Messages.RemoteResponse(); 
		try
		{
		                using (new OperationContextScope((IContextChannel)Proxy))
                {
				 response =  
					Proxy.GetData(remoteRequest);
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
		