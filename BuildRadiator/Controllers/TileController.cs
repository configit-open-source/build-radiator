using System.Collections.Generic;
using System.Web.Http;
using Configit.BuildRadiator.Model;

namespace Configit.BuildRadiator.Controllers {
  public class TileController: ApiController {
    private static readonly IReadOnlyCollection<Tile> Tiles;

    static TileController() {
      Tiles = new Tile[] {
        new ProjectTile( "Vcdb", "Vcdb_Commit" ),
        new ProjectTile( "Grid", "Grid_Commit" ),
        new MessageTile( "Current Theme", "sprintTheme", "fancy" ) { ColumnSpan = 2 }, 
        new ClockTile( "UK Time", "Europe/London" ),
        new DualProjectTile( "Ace Pricing (master)", "Deploy", "AcePricing_Commit", null, "AcePricing_Deploy_Daily", null ),

        new ProjectTile( "Ngyn", "Ngyn_Commit" ),
        new ProjectTile( "Licensing", "Licensing_Commit" ),
        new ProjectTile( "License Generator", "LicenseGenerator_Commit" ),
        new ProjectTile( "Installer", "Installer_Commit" ),
        new ProjectTile( "Database Comparer", "DatabaseComparer_Commit" ),
        new ProjectTile( "Database Installer", "DatabaseInstaller_Commit" ),

        new DualProjectTile( "Ace (master)", "Deploy", "Ace_Commit", null, "Ace_Deploy_Daily", null ),
        new DualProjectTile( "Ace (selene)", "Deploy", "Ace_Commit", "selene", "Ace_Deploy_Daily_Selene", null ),
        new DualProjectTile( "Ace (hydrogen)", "Deploy", "Ace_Commit", "hydrogen", "Ace_Deploy_Daily_Hydrogen", null ),
        new DualProjectTile( "Ace (indigo)", "Deploy", "Ace_Commit", "indigo", "Ace_Deploy_Daily_Indigo", null ),
        new ProjectTile( "Ace End To End", "Ace_Deploy_EndToEnd" ),
        new ProjectTile( "Ace Upgrade", "Ace_Deploy_DailyUpgrade" )
      };
    }

    public IEnumerable<Tile> Get() {
      return Tiles;
    }
  }
}
