using Configit.BuildRadiator.Model.Builds;

namespace Configit.BuildRadiator.Hubs {
  public interface IBuildHubClient {
    void Update( Build build );
    void UpdateError( BuildError buildError );
  }
}