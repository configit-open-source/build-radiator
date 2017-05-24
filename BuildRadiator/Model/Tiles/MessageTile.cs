using System.ComponentModel.DataAnnotations.Schema;

namespace Configit.BuildRadiator.Model {
  [Table( "MessageTiles" )]
  public class MessageTile: Tile {
    public Message Message { get; set; }
  }
}