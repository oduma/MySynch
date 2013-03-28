using System.ServiceModel;
using MySynch.Contracts.Messages;

namespace MySynch.Contracts
{
    public interface ISubscriberFeedback
    {

        [OperationContract(IsOneWay = true)]
        void SubscriberFeedback(SubscriberFeedbackMessage message);

    }
}
