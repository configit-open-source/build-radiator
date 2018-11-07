using Configit.BuildRadiator.Helpers;

namespace Configit.BuildRadiator.Model {
  public class DualProjectTile: Tile<DualProjectTileConfig> {
    public override string Type => "dual-project";

    public DualProjectTile( string caption, string secondaryCaption, string primaryBuildId, string primaryBranchName, string secondaryBuildId, string secondaryBranchName ) {
      Caption = caption;
      Config = new DualProjectTileConfig {
        SecondaryCaption = secondaryCaption,
        PrimaryBuildId = primaryBuildId,
        PrimaryBranchName = primaryBranchName,
        SecondaryBuildId = secondaryBuildId,
        SecondaryBranchName = secondaryBranchName
      };
    }
  }
}