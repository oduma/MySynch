using System;
using System.Collections.Generic;
using System.Linq;
using MySynch.Common.Logging;
using MySynch.Contracts.Messages;

namespace MySynch.Core.Broker
{
    public static class ExtensionMethods
    {
        public static List<Registration> AddRegistration(this List<Registration> original, Registration newRegistration)
        {
            if(original==null)
                original=new List<Registration>();
            original.Add(newRegistration);
            return original;
        }

        internal static List<Registration> RemoveRegistration(this List<Registration> original, string serviceUrl,out Registration deletedCopy)
        {
            if(original==null)
                original=new List<Registration>();
            var registration = original.FirstOrDefault(o => o.ServiceUrl == serviceUrl);
            if(registration==null)
            {
                deletedCopy = null;
                return original;
            }
            deletedCopy = new Registration
                              {
                                  OperationTypes = registration.OperationTypes,
                                  ServiceRole = registration.ServiceRole,
                                  ServiceUrl = registration.ServiceUrl
                              };
            original.Remove(registration);
            return original;
        }

        public static List<Registration> SaveAndReturn(this List<Registration> toSave, string storeName, Action<List<Registration>,string> saveMethod)
        {
            if(toSave==null)
                toSave= new List<Registration>();
            try
            {
                saveMethod(toSave, storeName);
                return toSave;

            }
            catch (Exception ex)
            {
                LoggingManager.LogMySynchSystemError(ex);
                throw;
            }
        }
    }
}
