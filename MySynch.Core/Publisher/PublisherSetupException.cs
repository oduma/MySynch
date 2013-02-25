using System;

namespace MySynch.Core.Publisher
{
    public class PublisherSetupException:Exception
    {
        public string SourceRootName { get; private set; }

        public PublisherSetupException(string sourceRootName, string message):base(message)
        {
            SourceRootName = sourceRootName;
        }
    }
}
