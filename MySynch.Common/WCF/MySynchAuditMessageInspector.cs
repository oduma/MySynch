using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using MySynch.Common.Logging;
using Enumerable = System.Linq.Enumerable;

namespace MySynch.Common.WCF
{
    public class MySynchAuditMessageInspector :
        IClientMessageInspector,
        IDispatchMessageInspector
    {
        private const string ConfigSwitch="CommunicationLogging";
        private bool? _configAllows = null;

        #region IClientMessageInspector Members

        /// <summary>
        /// This acts on the client only
        /// </summary>
        /// <param name="request"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            if(ConfigAllows())
                LoggingManager.Debug(request);
            return null;
        }

        /// <summary>
        /// This acts on the client only
        /// </summary>
        /// <param name="reply"></param>
        /// <param name="correlationState"></param>
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            if(ConfigAllows())
                LoggingManager.Debug(reply);
        }

        #endregion

        #region IDispatchMessageInspector Members

        /// <summary>
        /// This acts on the server only
        /// </summary>
        /// <param name="request"></param>
        /// <param name="channel"></param>
        /// <param name="instanceContext"></param>
        /// <returns></returns>
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            if(ConfigAllows())
            LoggingManager.Debug(request);
            return null;
        }

        /// <summary>
        /// This acts on the server only
        /// </summary>
        /// <param name="reply"></param>
        /// <param name="correlationState"></param>
        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            try
            {
                if(ConfigAllows())
                LoggingManager.Debug(reply);

            }
            catch
            {
                //swallow everything to return whatever comes from the server to client
            }
        }

        private bool ConfigAllows()
        {
            if(_configAllows.HasValue)
                return _configAllows.Value;
            var configAllows = Enumerable.FirstOrDefault<string>(ConfigurationManager.AppSettings.AllKeys, k => k == ConfigSwitch);
            if (string.IsNullOrEmpty(configAllows))
            {
                _configAllows = false;
                return _configAllows.Value;
            }
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings[ConfigSwitch]))
            {
                _configAllows= false;
                return _configAllows.Value;
            }
            if (ConfigurationManager.AppSettings[ConfigSwitch].ToLower() != "verbose")
            {
                _configAllows = false;
                return _configAllows.Value;
            }
            _configAllows = true;
            return  _configAllows.Value;
        }

        #endregion
    }
}
