using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using NodaTime;

namespace Configit.BuildRadiator.Model {
  [Table( "ClockTiles" )]
  public class ClockTile: Tile {
   
    private string _timeZoneId;

    public string TimeZoneId {
      get { return _timeZoneId; }
      set {
        if ( value == null ) throw new ArgumentNullException( nameof( value ) );

        if ( DateTimeZoneProviders
          .Tzdb[value]
          .GetZoneIntervals( Instant.FromUtc( System.DateTime.UtcNow.Year, 1, 1, 0, 0 ),
            Instant.FromUtc( System.DateTime.UtcNow.Year + 1, 1, 1, 0, 0 ) ).Any() ) {
          _timeZoneId = value;
        }
        else {
          throw new ArgumentException( nameof( value ) );
        }

      }
    }
  }
}