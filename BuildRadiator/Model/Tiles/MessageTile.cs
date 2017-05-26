using System.ComponentModel.DataAnnotations.Schema;

namespace Configit.BuildRadiator.Model {
  [Table( "MessageTiles" )]
  public class MessageTile: Tile {
    public string MessageKey { get; set; }
  }
}