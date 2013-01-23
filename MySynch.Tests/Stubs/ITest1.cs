using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace MySynch.Tests.Stubs
{
    [ServiceContract]
    internal interface ITest1
    {
        [OperationContract]
        void Opertaion1();
    }
}
