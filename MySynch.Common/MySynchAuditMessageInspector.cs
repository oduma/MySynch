using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace MySynch.Common
{
    public class MySynchAuditMessageInspector :
        IClientMessageInspector,
        IDispatchMessageInspector
    {
        #region IClientMessageInspector Members

        /// <summary>
        /// This acts on the client only
        /// </summary>
        /// <param name="request"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
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
                LoggingManager.Debug(reply);

            }
            catch
            {
                //swallow everything to return whatever comes from the server to client
            }
        }

        #endregion
    }
}
