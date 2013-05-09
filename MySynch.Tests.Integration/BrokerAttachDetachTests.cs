using MySynch.Contracts.Messages;
using MySynch.Tests.Integration.Helpers;
using NUnit.Framework;
using System.Collections.Generic;

namespace MySynch.Tests.Integration
{
    [TestFixture]
    [Ignore("These tests require the broker to be running on this machine")]
    public class BrokerAttachDetachTests
    {
        [Test]
        public void AttachToBrokerOk()
        {
            BrokerClientBuild clientBuild=new BrokerClientBuild();
            clientBuild.InitiateUsingServerAddress("http://sciendo-laptop/broker");
            AttachRequest request = new AttachRequest
                                        {
                                            RegistrationRequest =
                                                new Registration
                                                    {
                                                        MessageMethod = "My Method",
                                                        OperationTypes =
                                                            new List<OperationType>
                                                                {OperationType.Insert, OperationType.Update},
                                                        ServiceRole = ServiceRole.Publisher,
                                                        ServiceUrl = "my service url"
                                                    }
                                        };
            var response = clientBuild.Attach(request);
            Assert.True(response.RegisteredOk);
        }
        [Test]
        public void DetachToBrokerOk()
        {
            BrokerClientBuild clientBuild = new BrokerClientBuild();
            clientBuild.InitiateUsingServerAddress("http://sciendo-laptop/broker");
            DetachRequest request = new DetachRequest
            {
                        ServiceUrl = "my service url"
            };
            var response = clientBuild.Detach(request);
            Assert.True(response.Status);
        }
    }
}
