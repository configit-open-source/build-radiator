using System.ComponentModel.DataAnnotations.Schema;
using Configit.BuildRadiator.Helpers;
using Configit.BuildRadiator.Model.Builds;

namespace Configit.BuildRadiator.Model {
  [Table( "BuildTiles" )]
  public class BuildTile: Tile {

    public Build Build { get; set; }
  }
}