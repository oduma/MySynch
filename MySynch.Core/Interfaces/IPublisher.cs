using MySynch.Core.DataTypes;

namespace MySynch.Core.Interfaces
{
    public interface IPublisher:ICommunicationCopmonent
    {

        ChangePushPackage PublishPackage();

    }
}
