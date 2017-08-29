using System.Collections.Generic;
using System.Web.Http;

using Configit.BuildRadiator.Model;

namespace Configit.BuildRadiator.Controllers {
  public class TileController: ApiController {
    private static readonly IReadOnlyCollection<Tile> Tiles;

    static TileController() {
      Tiles = new Tile[] {
        new ProjectTile( "Ace (master)", "Ace Commit" ) { ColumnSpan = 2, RowSpan = 1 },
        new MessageTile( "Current Theme", "sprintTheme", "fancy" ) { ColumnSpan = 2 }, 
        new ClockTile( "UK Time", "Europe/London" ),
        new ProjectTile( "Ngyn", "Ngyn Commit" ),
        new ProjectTile( "Ace (Hydrogen)", "Ace Commit", "hydrogen" ),
        new ProjectTile( "Ace (Indigo)", "Ace Commit", "indigo" ),
        new ProjectTile( "Vcdb", "Vcdb Commit" ),
        new ProjectTile( "Installer", "Installer Commit" ),
        new ProjectTile( "Grid", "Grid Commit" ),
        new ProjectTile( "Database Installer", "Database Installer Commit" ),
        new ProjectTile( "Ace Daily (Product)", "Ace Daily Deploy" ),
        new ProjectTile( "Ace Daily (John Deere)", "Ace Daily Deploy (John Deere)" ),
        new ProjectTile( "Ace Daily (JLR)", "Ace Daily Deploy (JLR)" ),
        new ProjectTile( "Ace Daily (ABB)", "Ace Daily Deploy (ABB)" ),
        new ProjectTile( "Ace End To End", "Ace End To End Test" ),
        new ProjectTile( "Ace Upgrade", "Ace Daily Upgrade" )
      };
    }

    public IEnumerable<Tile> Get() {
      return Tiles;
    }
  }
}
