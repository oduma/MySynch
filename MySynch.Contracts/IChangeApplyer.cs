using System;
using System.ServiceModel;
using MySynch.Contracts.Messages;

namespace MySynch.Contracts
{
    [ServiceContract]
    public interface IChangeApplyer:ICommunicationCopmonent
    {
        [OperationContract]
        bool ApplyChangePackage(ChangePushPackage changePushPackage, string targetRootFolder,Func<string,string,bool> copyMethod);
    }
}
