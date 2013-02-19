using System;

namespace MySynch.Core
{
    public class SourceOfDataSetupException : Exception
    {
        public string Source { get; private set; }

        public SourceOfDataSetupException(string source, string message):base(message)
        {
            Source = source;
        }
    }
}
