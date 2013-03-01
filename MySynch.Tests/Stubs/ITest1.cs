using System.ServiceModel;
using MySynch.Contracts;

namespace MySynch.Tests.Stubs
{
    [ServiceContract]
    internal interface ITest1:ICommunicationComponent
    {
        [OperationContract]
        void Opertaion1();
    }
}
