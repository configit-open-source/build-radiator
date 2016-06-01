using Configit.BuildRadiator.Model;

namespace Configit.BuildRadiator.Hubs {
  public interface IMessageHubClient {
    void Update( Message message );
  }
}