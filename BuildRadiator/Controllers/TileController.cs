using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Http;
using System.Web.UI;
using Configit.BuildRadiator.Model;
using Configit.BuildRadiator.Model.Builds;
using Microsoft.Ajax.Utilities;

namespace Configit.BuildRadiator.Controllers {
  public class TileController: ApiController {

    private static readonly RadiatorContext Context;

    private static void InitializeDatabase() {
      Database.SetInitializer( new DropCreateDatabaseAlways<RadiatorContext>() );

      try {
        Context.Database.Initialize( true );

        var clockTile = new ClockTile() { Title = "Otter Time", TimeZoneId = "Europe/Copenhagen" };

        var message = new Message() { Key = "Otter", Content = "<img src='https://media.giphy.com/media/l0K45p4XQTVmyClMs/giphy-downsized-large.gif' />" };
        var message2 = new Message() { Key = "Log", Content = "<h1 style=\"color: orange;Height:200px ;background-image: url('http://p.fod4.com/p/media/413ade39e1/sCHdfjwxRui3uyB9h8mr_o3.gif'\">Otters rock!</H1>" };
        var message3 = new Message() { Key = "Hamster", Content = "<img src='https://68.media.tumblr.com/56ab2b83f9308cb58b5deb82c4c53cfd/tumblr_oie73yaAfM1rjlj53o1_500.gif' />" };
        Context.Messages.Add( message );
        Context.Messages.Add( message2 );
        Context.Messages.Add( message3);

        var messageTile = new MessageTile() { Title = "Important otter stuff", MessageKey = "Otter", ColumnSpan = 1 };
        var messageTile2 = new MessageTile() { Title = "Ottercam", MessageKey = "Log", ColumnSpan = 2 };

        var server = new BuildServer() { Password = "2vov3rap", Url = "http://configit.com", User = "Bob" };
        Context.BuildServers.Add( server );

        var build = new Build {
          BranchName = "selene",
          Name = "Ace Commit",
          Server = server
        };

        var build2 = new Build {
          BranchName = "selene",
          Name = "Ace Commit",
          Server = server
        };

        var buildTile = new BuildTile() { Title = "Build them all", Build = build };

        var tiles = new List<Tile>() { buildTile, clockTile,  messageTile2  }; //, , clockTile2

        var page1 = new Page() { Title = "Page 1", Tiles = tiles };
        page1.Tiles = tiles;

        Context.Pages.Add( page1 );

        Context.SaveChanges();
      }
      catch ( DbEntityValidationException ex ) {
        Console.WriteLine( ex.Message );
      }
    }

    static TileController() {
      Context = new RadiatorContext();
     // InitializeDatabase();
     // Context = new RadiatorContext();

      //StaticTiles = new Tile[] {
      //  new BuildTile() { ColumnSpan = 2, RowSpan = 1 },
      //  new MessageTile { ColumnSpan = 2 }, // "Current Theme", "sprintTheme", "fancy" ) { ColumnSpan = 2 }, 
      //  new ClockTile(), //( "UK Time", "Europe/London" ),
      //  new BuildTile(),// "Ngyn", "Ngyn Commit" ),
      //  new BuildTile(),// "Ace (Selene)", "Ace Commit", "selene" ),
      //  new BuildTile(),// "Ace (Carbon)", "Ace Commit (NUnit2)" ),
      //  new BuildTile(),// "Vcdb", "Vcdb Commit" ),
      //  new BuildTile(),// "Installer", "Installer Commit" ),
      //  new BuildTile(), // "Grid", "Grid Commit" ),
      //  new BuildTile(), // "Database Installer", "Database Installer Commit" ),
      //  new BuildTile(), // "Ace Daily (Product)", "Ace Daily Deploy" ),
      //  new BuildTile(), // "Ace Daily (John Deere)", "Ace Daily Deploy (John Deere)" ),
      //  new BuildTile(), // "Ace Daily (JLR)", "Ace Daily Deploy (JLR)" ),
      //  new BuildTile(), // "Ace Daily (ABB)", "Ace Daily Deploy (ABB)" ),
      //  new BuildTile(), // "Ace End To End", "Ace End To End Test" ),
      //  new BuildTile() // "Ace Upgrade", "Ace Daily Upgrade" )
      //};
    }


public IEnumerable<Tile> Get()
{

  var buildTiles = (IEnumerable<Tile>) Context.Tiles.OfType<BuildTile>().Include("Build");
  var messageTiles = (IEnumerable<Tile>) Context.Tiles.OfType<MessageTile>();
  var clockTiles = (IEnumerable<Tile>) Context.Tiles.OfType<ClockTile>();
  var tiles = buildTiles.Concat(messageTiles).Concat(clockTiles);

  return tiles?.ToList();

}

public class RadiatorContext: DbContext {
  public RadiatorContext() : base( "Radiator" ) {
  }
  //  public DbSet<User> Users { get; set; }
  public DbSet<Page> Pages { get; set; }

  public DbSet<Tile> Tiles { get; set; }
  public DbSet<BuildServer> BuildServers { get; set; }
  public DbSet<Build> Builds { get; set; }
  public DbSet<Message> Messages { get; set; }

  protected override void OnModelCreating( DbModelBuilder modelBuilder ) {

  }
}

public class Page {
  [Key]
  public int Id { get; set; }
  public String Title { get; set; }
  public List<Tile> Tiles { get; set; }
}

public class BuildServer {
  [Key]
  public int Id { get; set; }
  public string Url { get; set; }
  public string User { get; set; }
  public string Password { get; set; }
  //  public Build Build { get; set; }
}
  }
}
