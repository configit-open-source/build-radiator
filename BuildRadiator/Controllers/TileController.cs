using System.Collections.Generic;
using System.Web.Http;

using Configit.BuildRadiator.Model;

namespace Configit.BuildRadiator.Controllers {
  public class TileController: ApiController {
    private static readonly IReadOnlyCollection<Tile> Tiles;

    static TileController() {
      Tiles = new Tile[] {
        new ProjectTile( "Ace Commit", "Ace Commit", "master" ) { ColumnSpan = 2, RowSpan = 2 },
        new MessageTile( "Current Theme", "sprintTheme", "fancy" ) { ColumnSpan = 2 }, 
        new ClockTile( "UK Time", "Europe/London" ),
        new ProjectTile( "Ngyn Commit", "Ngyn Commit", "master" ),
        new ProjectTile( "Vcdb Commit", "Vcdb Commit", "master" ),
        new ProjectTile( "Installer Commit", "Installer Commit", "master" ),
        new ProjectTile( "Grid Commit", "Grid Commit", "master" ),
        new ProjectTile( "Base Commit", "Base Commit", "master" ),
        new ProjectTile( "Database Installer", "Database Installer Commit", "master" ),
        new ProjectTile( "Ace Carbon", "Ace Commit", "carbon" ),
        new ProjectTile( "Ace Daily (JLR)", "Ace Daily Deploy (JLR)", "master" ),
        new ProjectTile( "Ace Daily (Product)", "Ace Daily Deploy", "master" ),
        new ProjectTile( "Ace End To End", "Ace End To End Test", "master" ),
        new ProjectTile( "Ace Upgrade", "Ace Daily Upgrade", "master" ),
      };
    }

    public IEnumerable<Tile> Get() {
      return Tiles;
    }
  }
}