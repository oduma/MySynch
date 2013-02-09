using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySynch.Contracts;
using MySynch.Contracts.Messages;
using MySynch.Core.Interfaces;

namespace MySynch.Core
{
    public class RemoteCopier:ICopyStrategy
    {
        private ISourceOfData _sourceOfData;
        public bool Copy(string source, string target)
        {
            //using (RemoteResponse remoteResponse = _sourceOfData.GetData(new RemoteRequest{FileName=source}))
            //{
                
            //}
            return false;
        }

        public void Initialize(ISourceOfData sourceOfData)
        {
            throw new NotImplementedException();
        }
    }
}
