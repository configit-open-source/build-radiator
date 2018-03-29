namespace Configit.BuildRadiator.Model {
  public class MessageTile: Tile<MessageTileConfig> {
    public override string Type => "message";

    public MessageTile( string caption, string messageKey ) {
      Caption = caption;
      Config = new MessageTileConfig {
        MessageKey = messageKey
      };
    }
  }
}