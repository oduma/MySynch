using System;

namespace MySynch.Core
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
