using System.Collections.Generic;
using Castle.MicroKernel.Registration;
using MySynch.Core.DataTypes;

namespace MySynch.Core.Interfaces
{
    public interface IDistributor
    {
        List<AvailableChannel> AvailableChannels { get; }

        void DistributeMessages();

        void InitiateDistributionMap(string mapFile,IWindsorInstaller installer);
    }
}
