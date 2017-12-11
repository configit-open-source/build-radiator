namespace Configit.BuildRadiator.Model {
  public class DualProjectTileConfig {
    public string PrimaryBuildId { get; set; }
    public string PrimaryBranchName { get; set; }
    public string SecondaryBuildId { get; set; }
    public string SecondaryBranchName { get; set; }
    public string SecondaryCaption { get; set; }
  }
}