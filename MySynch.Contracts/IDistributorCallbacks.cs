using System.ServiceModel;
using MySynch.Contracts.Messages;

namespace MySynch.Contracts
{
    public interface IDistributorCallbacks
    {
        [OperationContract(IsOneWay = true)]
        void PackagePublished(ChangePushPackage changePushPackage);

        [OperationContract(IsOneWay = true)]
        void PackageApplyed(ChangePushPackage changePushPackage);
    }
}
