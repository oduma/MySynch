using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.Threading;
using MySynch.Common;
using MySynch.Common.Logging;
using MySynch.Common.WCF;

namespace MySynch.Core.WCF.Clients
{
    public class ChannelFactoryPool
    {
        private static readonly ClientSection _clientSection =
            (ClientSection)ConfigurationManager.GetSection("system.serviceModel/client");

        private readonly ReaderWriterLock _readerWriterLock = new ReaderWriterLock();
        private readonly Dictionary<string, ClientEndpoint> _clientEndpoints;

        #region SINGLETON

        private static readonly object _singletonSyncRoot = new object();
        private static volatile ChannelFactoryPool _instance;

        private ChannelFactoryPool()
        {
            _clientEndpoints = new Dictionary<string, ClientEndpoint>(_clientSection.Endpoints.Count);
        }

        public static ChannelFactoryPool Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_singletonSyncRoot)
                    {
                        if (_instance == null)
                            _instance = new ChannelFactoryPool();
                    }
                }

                return _instance;
            }
        }

        #endregion

        /// <summary>
        /// returns a channelFactory that connects to a service of contract type T
        /// identified by the given endpointName
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ChannelFactory<T> GetChannelFactory<T>(string endpointName)
        {
            LoggingManager.Debug("Trying to get channelfactory for endpoint: " + endpointName);

            ChannelFactory<T> channelFactory;

            if (!TryGetChannelFactory(endpointName, out channelFactory))
            {
                channelFactory = CreateAndCache<T>(endpointName);
            }
            LoggingManager.Debug("Got channel factory for:" + channelFactory.Endpoint.Address);

            return channelFactory;
        }

        /// <summary>
        /// Tries to populate the channelFactory out parameter if a channelFactory can be found
        /// for the contract type T identified by a enpointName.
        /// If there is not channelfactory for the type the parameter will be set to null
        /// and false will be returned
        /// The method uses a readlock, so many threads can call it at the same time
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channelFactory"></param>
        /// <returns></returns>
        private bool TryGetChannelFactory<T>(string endpointName, out ChannelFactory<T> channelFactory)
        {
            _readerWriterLock.AcquireReaderLock(1000);
            try
            {
                ClientEndpoint clientEndpoint;

                if (_clientEndpoints.TryGetValue(typeof(T).Name + endpointName, out clientEndpoint))
                {
                    EndpointChannelFactory endpointChannelFactory = clientEndpoint.Endpoint;

                    channelFactory = endpointChannelFactory.ChannelFactory as ChannelFactory<T>;

                    return true;
                }
                else
                {
                    channelFactory = null;
                    return false;
                }
            }
            finally
            {
                _readerWriterLock.ReleaseReaderLock();
            }
        }

        /// <summary>
        /// Will create the channelFactory and will cache for furthe calls
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private ChannelFactory<T> CreateAndCache<T>(string endpointName)
        {
            //Only one thread at a time can enter the upgradeable lock, 
            //and he has the right to upgrade to write
            //while in upgradeable mode, other threads can still read
            //- but he can change to write mode and block all readers
            _readerWriterLock.AcquireReaderLock(1000);

            try
            {
                ClientEndpoint clientEndpoint;
                EndpointChannelFactory endpointChannelFactory;

                if (_clientEndpoints.TryGetValue(typeof(T).Name + endpointName, out clientEndpoint))
                {
                    //already there so no need to create it
                    endpointChannelFactory = clientEndpoint.Endpoint;

                    return endpointChannelFactory.ChannelFactory as ChannelFactory<T>;
                }
                else
                {
                    //it doesn't exist so populate it, put it in the cache and return it
                    _readerWriterLock.UpgradeToWriterLock(1000);
                    try
                    {
                        clientEndpoint = GetClientEndpoint<T>(endpointName);

                        _clientEndpoints.Add(typeof(T).Name + endpointName, clientEndpoint);

                        endpointChannelFactory = clientEndpoint.Endpoint;

                        return endpointChannelFactory.ChannelFactory as ChannelFactory<T>;
                    }
                    finally
                    {
                        _readerWriterLock.ReleaseWriterLock();
                    }
                }
            }
            finally
            {
                _readerWriterLock.ReleaseLock();
            }
        }

        /// <summary>
        /// returns the ClientEndPoint for a specific contract and a specified name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private ClientEndpoint GetClientEndpoint<T>(string endpointName)
        {
            ChannelEndpointElement endpointConfigElement = null;
            var clientEndpoint = new ClientEndpoint(typeof(T),endpointName);
            for(int i=0;i<_clientSection.Endpoints.Count;i++)
            {
                if(_clientSection.Endpoints[i].Name==endpointName && _clientSection.Endpoints[i].Contract==typeof(T).FullName)
                {
                    endpointConfigElement = _clientSection.Endpoints[i];
                    
                }
            }
            var endpointChannelFactory = new EndpointChannelFactory
            {
                EndpointAddress = new EndpointAddress(endpointConfigElement.Address)
            };
            endpointChannelFactory.ChannelFactory = new ChannelFactory<T>(endpointConfigElement.Name,
                                                                          endpointChannelFactory.EndpointAddress);

            //endpointChannelFactory.ChannelFactory.Endpoint.Behaviors.Add(new ErgoSecurityBehavior());

            endpointChannelFactory.ChannelFactory.Endpoint.Behaviors.Add(new MySynchAuditBehavior());

            endpointChannelFactory.ChannelFactory.Open();

            clientEndpoint.Endpoint = endpointChannelFactory;

            return clientEndpoint;
        }

    }
}
