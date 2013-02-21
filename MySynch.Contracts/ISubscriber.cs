using System.Collections.Generic;
using System.ServiceModel;
using MySynch.Contracts.Messages;

namespace MySynch.Contracts
{
    [ServiceContract]
    public interface ISubscriber:ICommunicationComponent
    {
        [OperationContract]
        bool ApplyChangePackage(ChangePushPackage changePushPackage);

        [OperationContract]
        string GetTargetRootFolder();

        [OperationContract]
        bool TryOpenChannel(string sourceOfDataEndpointName);

        [OperationContract]
        IEnumerable<ChangePushPackage> GetDifferenceAsMessages(SynchItem currentPublisherRepository);
    }
}
