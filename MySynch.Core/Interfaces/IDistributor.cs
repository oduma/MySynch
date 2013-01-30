using System.Collections.Generic;
using MySynch.Contracts;
using MySynch.Contracts.Messages;
using MySynch.Core.DataTypes;

namespace MySynch.Core.Interfaces
{
    public interface IDistributor:IDistributorMonitor
    {
        List<AvailableChannel> AvailableChannels { get; }

        void DistributeMessages();

        void InitiateDistributionMap(string mapFile,ComponentResolver componentResolver);
    }
}
