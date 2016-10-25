using Configit.BuildRadiator.Helpers;

namespace Configit.BuildRadiator.Model {
  public class ProjectTile: Tile<ProjectTileConfig> {
    public override string Type => "project";

    public ProjectTile( string caption, string buildName, string branchName = null ) {
      Caption = caption;
      Config = new ProjectTileConfig {
        BuildName = buildName,
        BranchName = !string.IsNullOrWhiteSpace( branchName )
          ? branchName
          : BuildService.DefaultBranchName
      };
    }
  }
}