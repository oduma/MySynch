using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MySynch.Contracts.Messages
{
    [DataContract]
    public class PublishPackageRequestResponse
    {
        [DataMember]
        public string Source { get; set; }

        [DataMember]
        public string SourceRootName { get; set; }

        [DataMember]
        public List<ChangePushItem> ChangePushItems { get; set; }

        [DataMember]
        public Guid PackageId { get; set; }
    }
}
