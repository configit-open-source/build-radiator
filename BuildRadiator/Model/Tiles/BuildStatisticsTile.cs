using Configit.BuildRadiator.Helpers;

namespace Configit.BuildRadiator.Model {
  public class BuildStatisticsTile: Tile<BuildStatisticsTileConfig> {

    public BuildStatisticsTile( string caption, string buildName, string branchName = null ) {
      Caption = caption;
      Config = new BuildStatisticsTileConfig {
        BuildName = buildName,
        BranchName = !string.IsNullOrWhiteSpace( branchName )
          ? branchName
          : BuildStatisticsService.DefaultBranchName
      };
    }

    public override string Type => "build-statistics";
    
  }
}