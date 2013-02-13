using System;
using System.Collections.Generic;
using System.Configuration;
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


    }
}
