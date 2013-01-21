using MySynch.Core.DataTypes;

namespace MySynch.Core.Interfaces
{
    public interface ICommunicationCopmonent
    {
        string MachineName { get; }

        HeartbeatResponse GetHeartbeat();
    }
}
