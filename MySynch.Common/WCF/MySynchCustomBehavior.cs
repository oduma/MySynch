using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using MySynch.Common.WCF.Clients;

namespace MySynch.Common.WCF
{
    public class MySynchCustomBehavior : BehaviorExtensionElement, IEndpointBehavior
    {
        protected override object CreateBehavior()
        {
            return new MySynchCustomBehavior();
        }

        public override Type BehaviorType
        {
            get { return typeof(MySynchCustomBehavior); }
        }

        public void Validate(ServiceEndpoint endpoint)
        {
            return;
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            return;
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            //MySynchCustomServerMessageInspector inspector = new MySynchCustomServerMessageInspector();
            //endpointDispatcher.DispatchRuntime.MessageInspectors.Add(inspector);
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            MySynchCustomClientMessageInspector inspector = new MySynchCustomClientMessageInspector();
            clientRuntime.MessageInspectors.Add(inspector);
        }
    }
}
