using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using System.Threading;
using MySynch.Common;
using MySynch.Common.Logging;
using MySynch.Common.WCF;

namespace MySynch.Core.WCF.Clients.Discovery
{
    public class ChannelFactoryPool
    {
        private readonly ReaderWriterLock _readerWriterLock = new ReaderWriterLock();
        private readonly Dictionary<string, ClientEndpoint> _clientEndpoints;

        #region SINGLETON

        private static readonly object _singletonSyncRoot = new object();
        private static volatile ChannelFactoryPool _instance;

        private ChannelFactoryPool()
        {
            _clientEndpoints = new Dictionary<string, ClientEndpoint>();
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
        public ChannelFactory<T> GetChannelFactory<T>(int port)
        {
            LoggingManager.Debug("Trying to get channelfactory for service port: " + port);

            ChannelFactory<T> channelFactory;

            if (!TryGetChannelFactory(port, out channelFactory))
            {
                channelFactory = CreateAndCache<T>(port);
                if(channelFactory!=null)
                    LoggingManager.Debug("Got channel factory for:" + channelFactory.Endpoint.Address);
                else
                    LoggingManager.Debug("No endpoint found in the network listening to port: " + port);
            }
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
        private bool TryGetChannelFactory<T>(int port, out ChannelFactory<T> channelFactory)
        {
            _readerWriterLock.AcquireReaderLock(1000);
            try
            {
                ClientEndpoint clientEndpoint;

                if (_clientEndpoints.TryGetValue(typeof(T).Name + port, out clientEndpoint))
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
        private ChannelFactory<T> CreateAndCache<T>(int port)
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

                if (_clientEndpoints.TryGetValue(typeof(T).Name + port, out clientEndpoint))
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
                        EndpointAddress baseAddress;
                        if (FindService<T>(port, out baseAddress))
                        {
                            clientEndpoint = GetClientEndpoint<T>(typeof(T).Name + port,baseAddress);

                            _clientEndpoints.Add(typeof(T).Name + port, clientEndpoint);

                            endpointChannelFactory = clientEndpoint.Endpoint;
                            return endpointChannelFactory.ChannelFactory as ChannelFactory<T>;
                            
                        }
                        return null;
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

        internal bool FindService<T>(int port, out EndpointAddress baseAddress)
        {
            LoggingManager.Debug("Looking for service of type " + typeof(T).FullName + " listening at port: " + port);
            var services= DiscoveryHelper.FindServices<T>();
            if (services == null || services.Count(s => s.Address.Uri.Port == port) <= 0)
            {
                baseAddress = null;
                return false;
            }
            baseAddress = services.First(s => s.Address.Uri.Port == port).Address;
            return true;
        }

        /// <summary>
        /// returns the ClientEndPoint for a specific contract and a specified name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private ClientEndpoint GetClientEndpoint<T>(string clientEndpointName,EndpointAddress baseAddress)
        {
            var clientEndpoint = new ClientEndpoint(typeof(T), clientEndpointName);

            var endpointChannelFactory = new EndpointChannelFactory
                                             {
                                                 EndpointAddress = baseAddress
            };
            endpointChannelFactory.ChannelFactory = new ChannelFactory<T>(ClientServerBindingHelper.GetBinding(),
                                                                          endpointChannelFactory.EndpointAddress);
            endpointChannelFactory.ChannelFactory.Endpoint.Behaviors.Add(new MySynchAuditBehavior());

            endpointChannelFactory.ChannelFactory.Open();

            clientEndpoint.Endpoint = endpointChannelFactory;

            return clientEndpoint;
        }

    }
}
