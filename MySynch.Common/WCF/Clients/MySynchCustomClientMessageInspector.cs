﻿using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace MySynch.Common.WCF.Clients
{
    public class MySynchCustomClientMessageInspector : IClientMessageInspector
    {
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            request.Headers.Add(MessageHeader.CreateHeader("CustomClientMessageHeader", "", "something here"));
            return null;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            //throw new NotImplementedException();
        }
    }
}
