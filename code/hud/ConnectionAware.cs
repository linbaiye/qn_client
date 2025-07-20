using QnClient.code.network;

namespace QnClient.code.hud;

public interface IConnectionAware
{
    void SetConnection(Connection connection);
}