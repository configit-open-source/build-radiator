using System.Collections.Generic;
using System.Web.Http;

using Configit.BuildRadiator.Model;

namespace Configit.BuildRadiator.Controllers {
  public class TileController: ApiController {
    private static readonly IReadOnlyCollection<Tile> Tiles;

    static TileController() {
      Tiles = new Tile[] {
        new ProjectTile( "Ace Commit", "Ace Commit" ) { ColumnSpan = 2, RowSpan = 2 },
        new MessageTile( "Current Theme", "sprintTheme", "fancy" ) { ColumnSpan = 2 }, 
        new ClockTile( "UK Time", "Europe/London" ),
        new ProjectTile( "Ngyn Commit", "Ngyn Commit" ),
        new ProjectTile( "Vcdb Commit", "Vcdb Commit" ),
        new ProjectTile( "Installer Commit", "Installer Commit" ),
        new ProjectTile( "Grid Commit", "Grid Commit" ),
        new ProjectTile( "Base Commit", "Base Commit" ),
        new ProjectTile( "Database Installer", "Database Installer Commit" ),
        new ProjectTile( "Ace Carbon", "Ace Commit (NUnit2)" ),
        new ProjectTile( "Ace Daily (JLR)", "Ace Daily Deploy (JLR)" ),
        new ProjectTile( "Ace Daily (Product)", "Ace Daily Deploy" ),
        new ProjectTile( "Ace End To End", "Ace End To End Test" ),
        new ProjectTile( "Ace Upgrade", "Ace Daily Upgrade" )
      };
    }

    public IEnumerable<Tile> Get() {
      return Tiles;
    }
  }
}
