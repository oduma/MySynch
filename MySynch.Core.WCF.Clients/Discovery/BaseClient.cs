using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using MySynch.Common;
using MySynch.Common.Logging;

namespace MySynch.Core.WCF.Clients.Discovery
{
    public abstract class BaseClient<T> : IDisposable, IInitiateClient
    {
        private ICommunicationObject _channel;
        public T Proxy;

        protected virtual void OnTimeoutException(TimeoutException tex)
        {
            _channel.Abort();
            throw tex;
        }

        protected virtual void OnCommunicationException(CommunicationException cex)
        {
            _channel.Abort();
            throw cex;
        }

        protected virtual void OnException(Exception ex)
        {
            _channel.Abort();
            throw ex;
        }
        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                if (_channel != null)
                {
                    if (_channel.State != CommunicationState.Faulted)
                    {
                        _channel.Close();
                    }
                    else
                    {
                        _channel.Abort();
                    }
                }
            }
            catch (CommunicationException)
            {
                _channel.Abort();
            }
            catch (TimeoutException)
            {
                _channel.Abort();
            }
            catch (Exception)
            {
                _channel.Abort();
                throw;
            }
            finally
            {
                _channel = null;
            }
        }

        #endregion

        public void InitiateUsingPort(int port)
        {
            using (LoggingManager.LogMySynchPerformance())
            {

                ChannelFactory<T> channelFactory;
                try
                {
                    channelFactory = ChannelFactoryPool.Instance.GetChannelFactory<T>(port);
                }
                catch (Exception ex)
                {

                    throw ex;
                }

                Proxy = channelFactory.CreateChannel();

                _channel = (ICommunicationObject)Proxy;
            }
        }
    }
}
