using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using MySynch.Common;
using MySynch.Core.DataTypes;

namespace MySynch.Core
{
    public static class ServiceHelper
    {

        public static List<RoleOfNode> DetermineRolesOfNode(string nodeRolesConfigKeyName)
        {
            LoggingManager.Debug("Determining node roles");
            if (!ConfigurationManager.AppSettings.AllKeys.Contains(nodeRolesConfigKeyName))
            {
                LoggingManager.Debug("This node has no role defined in the configuration");
                return new List<RoleOfNode>();
            }
            var configSettings = ConfigurationManager.AppSettings[nodeRolesConfigKeyName].Split(new char[] { ',' });
            return configSettings.Select((c) =>
            {
                RoleOfNode roleNode;
                try
                {
                    Enum.TryParse(c, true, out roleNode);
                    return roleNode;

                }
                catch (Exception ex)
                {
                    LoggingManager.LogMySynchSystemError("While trying to process role named:" + c, ex);
                    return RoleOfNode.None;
                }
            }).ToList();
        }

        public static List<RoleOfNode> ListVerifiedRolesOfNode(string nodeRolesConfigKeyName, string nodeMapFileName)
        {
            LoggingManager.Debug("Starting Verifying the configuration of the current node");
            if (string.IsNullOrEmpty(nodeMapFileName))
            {
                LoggingManager.Debug("No mapfile sent. Nothing to do.");
                return new List<RoleOfNode>();
            }
            if (!File.Exists(nodeMapFileName))
            {
                LoggingManager.Debug("Map file not found. Nothing to do.");
                return new List<RoleOfNode>();
            }
            var rolesOfNode = DetermineRolesOfNode(nodeRolesConfigKeyName);
            if(rolesOfNode.Count==0)
                return rolesOfNode;
            if (rolesOfNode.Count(r => r != RoleOfNode.None) == 0)
            {
                LoggingManager.Debug("This Node has no usable role in its configuration");
                return new List<RoleOfNode>();
            }
            rolesOfNode = rolesOfNode.Distinct().ToList();
            try
            {
                var nodeMap = Serializer.DeserializeFromFile<AvailableChannel>(nodeMapFileName);
                if (nodeMap.Count == 0)
                {
                    LoggingManager.Debug("Node map does not contain any channels. Ntohing to do.");
                    return new List<RoleOfNode>();
                }
                if (nodeMap.Count(n => string.IsNullOrEmpty(n.PublisherInfo.EndpointName)) == 0)
                    //there is no local publisher so it cannot be a publisher
                    rolesOfNode.Remove(RoleOfNode.Publisher);
                if (nodeMap.Count(n => string.IsNullOrEmpty(n.SubscriberInfo.EndpointName)) == 0)
                    //there is no local subscriber so it cannot be a subscriber
                    rolesOfNode.Remove(RoleOfNode.Subscriber);
                return rolesOfNode;
            }
            catch (Exception ex)
            {
                LoggingManager.LogMySynchSystemError(ex);
                return new List<RoleOfNode>();
            }
        }
    }
}
