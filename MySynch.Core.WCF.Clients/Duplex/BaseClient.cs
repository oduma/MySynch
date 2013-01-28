using System;
using System.ServiceModel;

namespace MySynch.Core.WCF.Clients.Duplex
{
    public abstract class BaseClient<T, TCallback> : IDisposable, IInitiateClient<TCallback>
    {
        private ICommunicationObject _channel;
        public T Proxy;

        public void InitiateUsingEndpoint(TCallback callbackInstance, string endpointName)
        {
            ChannelFactory<T> channelFactory;
            try
            {
                channelFactory = ChannelFactoryPool<T>.Instance.GetChannelFactory<TCallback>(callbackInstance, endpointName);
            }
            catch (Exception ex)
            {

                throw ex;
            }

            Proxy = channelFactory.CreateChannel();

            _channel = (ICommunicationObject)Proxy;
        }

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
    }
}
