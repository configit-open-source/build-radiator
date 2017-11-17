using System.Collections.Generic;
using System.Web.Http;
using Configit.BuildRadiator.Model;

namespace Configit.BuildRadiator.Controllers {
  public class TileController: ApiController {
    private static readonly IReadOnlyCollection<Tile> Tiles;

    static TileController() {
      Tiles = new Tile[] {
        new ProjectTile( "Ace (master)", "Ace_Commit" ) { ColumnSpan = 2, RowSpan = 1 },
        new MessageTile( "Current Theme", "sprintTheme", "fancy" ) { ColumnSpan = 2 }, 
        new ClockTile( "UK Time", "Europe/London" ),
        new ProjectTile( "Ngyn", "Ngyn_Commit" ),
        new ProjectTile( "Grid", "Grid_Commit" ),
        new ProjectTile( "Vcdb", "Vcdb_Commit" ),
        new ProjectTile( "Licensing", "Licensing_Commit" ),
        new ProjectTile( "Installer", "Installer_Commit" ),
        new ProjectTile( "Database Installer", "DatabaseInstaller_Commit" ),
        new ProjectTile( "Ace Upgrade", "Ace_Deploy_DailyUpgrade" ),
        new ProjectTile( "Ace (Selene)", "Ace_Commit", "selene" ),
        new ProjectTile( "Ace (Hydrogen)", "Ace_Commit", "hydrogen" ),
        new ProjectTile( "Ace (Indigo)", "Ace_Commit", "indigo" ),
        new ProjectTile( "Ace Daily", "Ace_Deploy_Daily" ),
        new ProjectTile( "Ace End To End", "Ace_Deploy_EndToEnd" ),
        new ProjectTile( "Pricing Daily", "AcePricing_DailyDeploy" )
      };
    }

    public IEnumerable<Tile> Get() {
      return Tiles;
    }
  }
}
