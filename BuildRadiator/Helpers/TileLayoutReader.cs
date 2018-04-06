using System.Collections.Generic;
using System.IO;
using Configit.BuildRadiator.Model;
using Newtonsoft.Json.Linq;

namespace Configit.BuildRadiator.Helpers {
  internal class TileLayoutReader {
    public IReadOnlyList<Tile> Deserialize( string tileLayoutFile ) {
      var tiles = new List<Tile>();

      var layoutJson = JObject.Parse( File.ReadAllText( tileLayoutFile ) );

      foreach ( var tile in layoutJson.GetValue( "tiles" ).Children<JObject>() ) {

        var type = tile.Value<string>( "type" );


        switch ( type ) {
          case "project":
            tiles.Add( DeserializeProjectTile( tile ) );
            break;
          case "dual-project":
            tiles.Add( DeserializeDualProjectTile( tile ) );
            break;
          case "clock":
            tiles.Add( DeserializeClockTile( tile ) );
            break;
          case "message":
            tiles.Add( DeserializeMessageTile( tile ) );
            break;
        }

      }

      return tiles;
    }

    private static MessageTile DeserializeMessageTile( JObject tile ) {
      var caption = tile.Value<string>( "caption" );
      var tileConfig = tile.Value<JObject>( "config" );

      var messageKey = tileConfig.Value<string>( "messageKey" );

      var messageTile = new MessageTile( caption, messageKey );

      messageTile.ApplyStandardProperties( tile );

      return messageTile;
    }

    private static ClockTile DeserializeClockTile( JObject tile ) {
      var caption = tile.Value<string>( "caption" );
      var tileConfig = tile.Value<JObject>( "config" );

      var timezone = tileConfig.Value<string>( "timezone" );

      var clockTile = new ClockTile( caption, timezone );

      clockTile.ApplyStandardProperties( tile );

      return clockTile;
    }

    private static DualProjectTile DeserializeDualProjectTile( JObject tile ) {
      var caption = tile.Value<string>( "caption" );
      var tileConfig = tile.Value<JObject>( "config" );

      var secondaryCaption = tileConfig.Value<string>( "secondaryCaption" );
      var primaryBuildId = tileConfig.Value<string>( "primaryBuildId" );
      var primaryBranchName = tileConfig.Value<string>( "primaryBranchName" );
      var secondaryBuildId = tileConfig.Value<string>( "secondaryBuildId" );
      var secondaryBranchName = tileConfig.Value<string>( "secondaryBranchName" );

      var dualProjectTile = new DualProjectTile( caption, secondaryCaption, primaryBuildId, primaryBranchName, secondaryBuildId, secondaryBranchName );

      dualProjectTile.ApplyStandardProperties( tile );

      return dualProjectTile;
    }

    private static ProjectTile DeserializeProjectTile( JObject tile ) {
      var caption = tile.Value<string>( "caption" );
      var tileConfig = tile.Value<JObject>( "config" );

      var buildId = tileConfig.Value<string>( "buildId" );
      var branchName = tileConfig.Value<string>( "branchName" );

      var projectTile = new ProjectTile( caption, buildId, branchName );

      projectTile.ApplyStandardProperties( tile );

      return projectTile;
    }
  }
}