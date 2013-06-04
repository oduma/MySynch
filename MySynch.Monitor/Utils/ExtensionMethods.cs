using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using MySynch.Contracts.Messages;
using MySynch.Monitor.MVVM.ViewModels;

namespace MySynch.Monitor.Utils
{
    public static class ExtensionMethods
    {
        internal static ObservableCollection<RegistrationModel> ConvertToObservableCollection(this List<Registration> registrations)
        {
            ObservableCollection<RegistrationModel> registrationModels= new ObservableCollection<RegistrationModel>();
            foreach (Registration registration in registrations)
            {
                registrationModels.Add(new RegistrationModel
                                           {
                                               Operations =
                                                   string.Join(",",
                                                               registration.OperationTypes.Select(o => o.ToString())),
                                               ServiceRole = registration.ServiceRole,
                                               ServiceUrl = registration.ServiceUrl
                                           });
            }
            return registrationModels;
        }
    }
}
