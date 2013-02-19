using System;
using System.IO;
using MySynch.Common;
using MySynch.Contracts;
using MySynch.Contracts.Messages;

namespace MySynch.Core
{
    public class RemoteSourceOfData:ISourceOfData
    {
        public RemoteResponse GetData(RemoteRequest remoteRequest)
        {
            if(remoteRequest==null || string.IsNullOrEmpty(remoteRequest.FileName))
                throw new ArgumentNullException("remoteRequest");
            if(!File.Exists(remoteRequest.FileName))
                throw new ArgumentException("File does not exist " + remoteRequest.FileName);
            LoggingManager.Debug("Using remote datasource returning contents of file: " + remoteRequest.FileName); 
            RemoteResponse response = new RemoteResponse();
            FileInfo fInfo= new FileInfo(remoteRequest.FileName);
            response.Data= new byte[fInfo.Length];
            using (FileStream stream = fInfo.OpenRead())
            {
                stream.Read(response.Data, 0, response.Data.Length);
                stream.Flush();
            }
            return response;
        }

        public HeartbeatResponse GetHeartbeat()
        {
            return new HeartbeatResponse {Status = true};
        }
    }
}
