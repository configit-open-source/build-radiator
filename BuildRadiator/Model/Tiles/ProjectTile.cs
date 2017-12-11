using Configit.BuildRadiator.Helpers;

namespace Configit.BuildRadiator.Model {
  public class ProjectTile: Tile<ProjectTileConfig> {
    public override string Type => "project";

    public ProjectTile( string caption, string buildId, string branchName = null ) {
      Caption = caption;
      Config = new ProjectTileConfig {
        BuildId = buildId,
        BranchName = !string.IsNullOrWhiteSpace( branchName ) ? branchName : BuildService.DefaultBranchName
      };
    }
  }
}