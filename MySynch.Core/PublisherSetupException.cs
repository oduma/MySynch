using System;

namespace MySynch.Core
{
    public class PublisherSetupException:Exception
    {
        public string Source { get; private set; }

        public string SourceRootName { get; private set; }

        public PublisherSetupException(string source, string sourceRootName, string message):base(message)
        {
            Source = source;
            SourceRootName = sourceRootName;
        }
    }
}
