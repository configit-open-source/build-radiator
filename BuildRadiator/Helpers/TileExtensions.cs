using Configit.BuildRadiator.Model;
using Newtonsoft.Json.Linq;

namespace Configit.BuildRadiator.Helpers {
  internal static class TileExtensions {
    internal static void ApplyStandardProperties( this Tile tileObject, JObject tile ) {
      var columnSpan = tile.Value<int?>( "columnSpan" );
      var rowSpan = tile.Value<int?>( "rowSpan" );

      tileObject.ColumnSpan = columnSpan.GetValueOrDefault( 1 );
      tileObject.RowSpan = rowSpan.GetValueOrDefault( 1 );
    }
  }
}