using System;
using System.ServiceModel;
using MySynch.Contracts.Messages;

namespace MySynch.Contracts
{
    [ServiceContract]
    public interface ISubscriber:ICommunicationComponent
    {
        [OperationContract]
        bool ApplyChangePackage(ChangePushPackage changePushPackage,string sourceOfDataEndPointName=null);

        [OperationContract]
        string GetTargetRootFolder();
    }
}
