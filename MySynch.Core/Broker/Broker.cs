using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using MySynch.Common.IOC;
using MySynch.Common.Logging;
using MySynch.Common.Serialization;
using MySynch.Contracts;
using MySynch.Contracts.Messages;

namespace MySynch.Core.Broker
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Broker:IBroker
    {
        private IEnumerable<Registration> _registrations;
        private StoreType _store;
        private IStore<Registration> _storeHandler;

        public GetHeartbeatResponse GetHeartbeat()
        {
            return new GetHeartbeatResponse {Status = true};
        }

        public Broker(StoreType storeType, MySynchComponentResolver componentResolver)
        {
            try
            {
                _store = storeType;
                _storeHandler = componentResolver.Resolve<IStore<Registration>>(storeType.StoreTypeName);
                _registrations =
                    _storeHandler.GetMethod(_store.StoreName);

            }
            catch (ComponentNotRegieteredException ex)
            {
                LoggingManager.LogMySynchSystemError(ex);
                throw;
            }
            catch (Exception ex)
            {
                LoggingManager.LogMySynchSystemError(ex);
                _registrations = new List<Registration>();

            }
        }

        public AttachResponse Attach(AttachRequest request)
        {
            using (LoggingManager.LogMySynchPerformance())
            {
                if (request == null || request.RegistrationRequest == null ||
                    string.IsNullOrEmpty(request.RegistrationRequest.ServiceUrl))
                    return new AttachResponse {RegisteredOk = false};
                try
                {
                    if (_registrations.Any(r => r.ServiceUrl == request.RegistrationRequest.ServiceUrl))
                        return new AttachResponse {RegisteredOk = true};
                    _registrations =
                        _registrations.ToList().AddRegistration(request.RegistrationRequest).SaveAndReturn(
                            _store.StoreName, _storeHandler.StoreMethod);
                    return new AttachResponse {RegisteredOk = true};
                }
                catch (Exception ex)
                {
                    LoggingManager.LogMySynchSystemError(ex);
                    return new AttachResponse {RegisteredOk = false};
                }
            }
        }

        public DetachResponse Detach(DetachRequest request)
        {
            if(request==null || string.IsNullOrEmpty(request.ServiceUrl))
                return new DetachResponse{Status = false};
            try
            {
                if (!_registrations.Any(r => r.ServiceUrl == request.ServiceUrl))
                    return new DetachResponse { Status = false };
                _registrations =
                    _registrations.ToList().RemoveRegistration(request.ServiceUrl).SaveAndReturn(
                        _store.StoreName,_storeHandler.StoreMethod);
                return new DetachResponse { Status = true };
            }
            catch (Exception ex)
            {
                LoggingManager.LogMySynchSystemError(ex);
                return new DetachResponse { Status = false };
            }
        }

        public ListAllRegistrationsResponse ListAllRegistrations()
        {
            return new ListAllRegistrationsResponse {Registrations = _registrations.ToList()};
        }
    }
}
