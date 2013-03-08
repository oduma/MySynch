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
	public class DistributorMonitorClient :BaseClient<IDistributorMonitor>,IDistributorMonitorProxy
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
		public MySynch.Contracts.Messages.ListAvailableComponentsTreeResponse ListAvailableComponentsTree() 
		{
		MySynch.Contracts.Messages.ListAvailableComponentsTreeResponse response = new MySynch.Contracts.Messages.ListAvailableComponentsTreeResponse(); 
		try
		{
		                using (new OperationContextScope((IContextChannel)Proxy))
                {
				 response =  
					Proxy.ListAvailableComponentsTree();
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

			return ; 

		}
		public MySynch.Contracts.Messages.GetCurrentMapResponse GetCurrentMap() 
		{
		MySynch.Contracts.Messages.GetCurrentMapResponse response = new MySynch.Contracts.Messages.GetCurrentMapResponse(); 
		try
		{
		                using (new OperationContextScope((IContextChannel)Proxy))
                {
				 response =  
					Proxy.GetCurrentMap();
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
		