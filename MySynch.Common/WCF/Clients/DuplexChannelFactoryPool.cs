using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using MySynch.Common.Logging;

namespace MySynch.Common.WCF.Clients
{
    class DuplexChannelFactoryPool<T>
    {
        private readonly ReaderWriterLock _readerWriterLock = new ReaderWriterLock();
        internal readonly Dictionary<string, DuplexClientEndpoint<T>> ClientEndpoints;

        #region SINGLETON

        private static readonly object _singletonSyncRoot = new object();
        private static volatile DuplexChannelFactoryPool<T> _instance;

        private DuplexChannelFactoryPool()
        {
            ClientEndpoints = new Dictionary<string, DuplexClientEndpoint<T>>();
        }

        public static DuplexChannelFactoryPool<T> Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_singletonSyncRoot)
                    {
                        if (_instance == null)
                            _instance = new DuplexChannelFactoryPool<T>();
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
        public DuplexChannelFactory<T> GetChannelFactory(string serverAddress,List<IEndpointBehavior> endpointBehaviors, InstanceContext callbackInstance)
        {
            LoggingManager.Debug("Trying to get channelfactory for endpoint: " + serverAddress);

            DuplexChannelFactory<T> channelFactory;

            if (!TryGetChannelFactory(serverAddress, out channelFactory))
                channelFactory = CreateAndCache(serverAddress,endpointBehaviors,callbackInstance);
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
        private bool TryGetChannelFactory(string serverAddress, out DuplexChannelFactory<T> channelFactory)
        {
            _readerWriterLock.AcquireReaderLock(1000);
            try
            {
                DuplexClientEndpoint<T> clientEndpoint;

                if (ClientEndpoints.TryGetValue(typeof(T).Name + serverAddress, out clientEndpoint))
                {
                    DuplexEndpointChannelFactory<T> endpointChannelFactory = clientEndpoint.Endpoint;

                    channelFactory = endpointChannelFactory.ChannelFactory as DuplexChannelFactory<T>;

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
        private DuplexChannelFactory<T> CreateAndCache(string serverAddress,List<IEndpointBehavior> endpointBehaviors, InstanceContext callbackInstance)
        {
            //Only one thread at a time can enter the upgradeable lock, 
            //and he has the right to upgrade to write
            //while in upgradeable mode, other threads can still read
            //- but he can change to write mode and block all readers
            _readerWriterLock.AcquireReaderLock(1000);

            try
            {
                DuplexClientEndpoint<T> clientEndpoint;
                DuplexEndpointChannelFactory<T> endpointChannelFactory;

                if (ClientEndpoints.TryGetValue(typeof(T).Name + serverAddress, out clientEndpoint))
                {
                    //already there so no need to create it
                    endpointChannelFactory = clientEndpoint.Endpoint;

                    return endpointChannelFactory.ChannelFactory as DuplexChannelFactory<T>;
                }
                else
                {
                    //it doesn't exist so populate it, put it in the cache and return it
                    _readerWriterLock.UpgradeToWriterLock(1000);
                    try
                    {
                        clientEndpoint = GetClientEndpoint<T>(serverAddress,endpointBehaviors,callbackInstance);

                        ClientEndpoints.Add(typeof(T).Name + serverAddress, clientEndpoint);

                        endpointChannelFactory = clientEndpoint.Endpoint;

                        return endpointChannelFactory.ChannelFactory as DuplexChannelFactory<T>;
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
        private static DuplexClientEndpoint<T> GetClientEndpoint<T>(string serverAddress,List<IEndpointBehavior> endpointBehaviors, InstanceContext callbackInstance)
        {
            EndpointAddress baseAddress= new EndpointAddress(serverAddress);

            var clientEndpoint = new DuplexClientEndpoint<T>(typeof(T), serverAddress);

            var endpointChannelFactory = new DuplexEndpointChannelFactory<T>
            {
                EndpointAddress = baseAddress
            };
            endpointChannelFactory.ChannelFactory = new DuplexChannelFactory<T>(callbackInstance,ClientServerBindingHelper.GetBinding(true).ApplyClientWsDualHttpBinding(),
                                                                          endpointChannelFactory.EndpointAddress);
            foreach(var endpointBehavior in endpointBehaviors)
                endpointChannelFactory.ChannelFactory.Endpoint.Behaviors.Add(endpointBehavior);

            endpointChannelFactory.ChannelFactory.Open();

            clientEndpoint.Endpoint = endpointChannelFactory;

            return clientEndpoint;
        }


    }
}
