using System;
using MySynch.Contracts;
using MySynch.Contracts.Messages;
using MySynch.Core.Distributor;

namespace MySynch.Tests.Stubs
{
    public class FakeSubscriberFeedback:ISubscriberFeedback
    {
        public Distributor DistributorInstance { get; set; }

        public void SubscriberFeedback(SubscriberFeedbackMessage message)
        {
            DistributorInstance.StillProcessing = false;
            DistributorInstance.AllProcessedOk = true;
        }
    }
}
