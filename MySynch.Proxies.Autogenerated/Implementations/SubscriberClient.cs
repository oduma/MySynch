//This code was auto generated with a tool
//do not change this file
using System;
using System.ServiceModel;
using MySynch.Contracts;
using MySynch.Common.WCF;
using MySynch.Common.WCF.Clients;
using MySynch.Proxies.Autogenerated.Interfaces;
using System.ServiceModel.Description;
using System.Collections.Generic;


namespace MySynch.Proxies.Autogenerated.Implementations
{
	public class SubscriberClient :BaseClient<ISubscriber>,ISubscriberProxy
	{
		protected override System.Collections.Generic.List<System.ServiceModel.Description.IEndpointBehavior> GetEndpointBehaviors()
        {
            return new List<IEndpointBehavior>
			{
							new MySynchAuditBehavior()
			};
        }

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
		public MySynch.Contracts.Messages.ReceiveMessageResponse ReceiveMessage(MySynch.Contracts.Messages.ReceiveMessageRequest request) 
		{
		MySynch.Contracts.Messages.ReceiveMessageResponse response = new MySynch.Contracts.Messages.ReceiveMessageResponse(); 
		try
		{
		                using (new OperationContextScope((IContextChannel)Proxy))
                {
				 response =  
					Proxy.ReceiveMessage(request);
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
		