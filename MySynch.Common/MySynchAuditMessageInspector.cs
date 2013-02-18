using System;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace MySynch.Common
{
    public class MySynchAuditMessageInspector :
        IClientMessageInspector,
        IDispatchMessageInspector
    {
        private const string ConfigSwitch="CommunicationLogging";

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
            var configAllows = ConfigurationManager.AppSettings.AllKeys.FirstOrDefault(k => k == ConfigSwitch);
            if (string.IsNullOrEmpty(configAllows))
                return false;
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings[ConfigSwitch]))
                return false;
            if (ConfigurationManager.AppSettings[ConfigSwitch].ToLower() != "verbose")
                return false;
            return true;
        }

        #endregion
    }
}
