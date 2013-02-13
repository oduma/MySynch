using MySynch.Core;
using MySynch.Core.DataTypes;
using MySynch.WindowsService;
using NUnit.Framework;
using System.Linq;

namespace MySynch.Tests
{
    [TestFixture]
    public class InitiateServiceTests
    {
        [Test]
        public void DetermineNodeRoles_Ok()
        {
            var rolesOfNode =ServiceHelper.DetermineRolesOfNode("NodeRoles");
            Assert.IsNotNull(rolesOfNode);
            Assert.AreEqual(3,rolesOfNode.Count);
            Assert.Contains(RoleOfNode.Distributor,rolesOfNode);
            Assert.Contains(RoleOfNode.Publisher,rolesOfNode);
            Assert.Contains(RoleOfNode.Subscriber,rolesOfNode);
            Assert.AreEqual(0,rolesOfNode.Count(r=>r==RoleOfNode.None));
        }

        [Test]
        public void DetermineNodeRoles_NoConfiguration()
        {
            var rolesOfNode = ServiceHelper.DetermineRolesOfNode("DoesNotExist");
            Assert.IsNotNull(rolesOfNode);
            Assert.AreEqual(0,rolesOfNode.Count);
        }

        [Test]
        public void DetermineNodeRoles_PartialConfiguration()
        {
            var rolesOfNode = ServiceHelper.DetermineRolesOfNode("PublisherOnly");
            Assert.IsNotNull(rolesOfNode);
            Assert.AreEqual(1, rolesOfNode.Count);
            Assert.Contains(RoleOfNode.Publisher, rolesOfNode);
            Assert.AreEqual(0, rolesOfNode.Count(r => r != RoleOfNode.Publisher));

            rolesOfNode = ServiceHelper.DetermineRolesOfNode("DistributorOnly");
            Assert.IsNotNull(rolesOfNode);
            Assert.AreEqual(1, rolesOfNode.Count);
            Assert.Contains(RoleOfNode.Distributor, rolesOfNode);
            Assert.AreEqual(0, rolesOfNode.Count(r => r != RoleOfNode.Distributor));

            rolesOfNode = ServiceHelper.DetermineRolesOfNode("SubscriberOnly");
            Assert.IsNotNull(rolesOfNode);
            Assert.AreEqual(1, rolesOfNode.Count);
            Assert.Contains(RoleOfNode.Subscriber, rolesOfNode);
            Assert.AreEqual(0, rolesOfNode.Count(r => r != RoleOfNode.Subscriber));

        }

        [Test]
        public void DetermineNodeRoles_WrongValue()
        {
            var rolesOfNode = ServiceHelper.DetermineRolesOfNode("WrongValue");
            Assert.IsNotNull(rolesOfNode);
            Assert.AreEqual(3, rolesOfNode.Count);
            Assert.AreEqual(2, rolesOfNode.Count(r=>r==RoleOfNode.None));
            Assert.AreEqual(1, rolesOfNode.Count(r=>r==RoleOfNode.Distributor));
        }
    }
}
