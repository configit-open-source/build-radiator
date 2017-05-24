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

namespace Configit.BuildRadiator.Controllers {
  public class TileController: ApiController {
    private static readonly IReadOnlyCollection<Tile> StaticTiles;
    private readonly RadiatorContext Context;

    public TileController() {
      Database.SetInitializer( new DropCreateDatabaseAlways<RadiatorContext>() );
      Context = new RadiatorContext();

      try {

        Context.Database.Initialize( true );


        var clockTile = new ClockTile() { Title = "Copenhagen Time", TimeZoneId = "Europe/Copenhagen"};
        var clockTile2 = new ClockTile() { Title = "UK time", TimeZoneId = "Europe/London"};
        //db.Tiles.Add( clockTile );

        var message = new Message() { Key = "Otter", Content = "Squirrels!" };
        var messageTile = new MessageTile() { Title = "Message", Message = message };
        //   db.Tile.Add( messageTile );



        var build = new Build {
          BranchName = "Twig",
          Committers = { "", "" },
          Start = DateTime.Now.Subtract( new TimeSpan( 0, 1, 15, 18 ) ),
          End = DateTime.Now.Subtract( new TimeSpan( 0, 0, 29, 36 ) ),
          Status = BuildStatus.Success,
          StatusText = "Miaow",
          Name = "Lovely Build",
          PercentComplete = 100
        };
        var buildTile = new BuildTile() { Title = "Build them all", Build = build };
        //db.Tiles.Add( buildTile );
        var server = new BuildServer() { Password = "otterz", Url = "http://configit.com", User = "Bob" };
        Context.BuildServers.Add( server );


        var tiles = new List<Tile>() { buildTile, clockTile, messageTile, clockTile2 };

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
      StaticTiles = new Tile[] {
        new BuildTile() { ColumnSpan = 2, RowSpan = 1 },
        new MessageTile { ColumnSpan = 2 }, // "Current Theme", "sprintTheme", "fancy" ) { ColumnSpan = 2 }, 
        new ClockTile(), //( "UK Time", "Europe/London" ),
        new BuildTile(),// "Ngyn", "Ngyn Commit" ),
        new BuildTile(),// "Ace (Selene)", "Ace Commit", "selene" ),
        new BuildTile(),// "Ace (Carbon)", "Ace Commit (NUnit2)" ),
        new BuildTile(),// "Vcdb", "Vcdb Commit" ),
        new BuildTile(),// "Installer", "Installer Commit" ),
        new BuildTile(), // "Grid", "Grid Commit" ),
        new BuildTile(), // "Database Installer", "Database Installer Commit" ),
        new BuildTile(), // "Ace Daily (Product)", "Ace Daily Deploy" ),
        new BuildTile(), // "Ace Daily (John Deere)", "Ace Daily Deploy (John Deere)" ),
        new BuildTile(), // "Ace Daily (JLR)", "Ace Daily Deploy (JLR)" ),
        new BuildTile(), // "Ace Daily (ABB)", "Ace Daily Deploy (ABB)" ),
        new BuildTile(), // "Ace End To End", "Ace End To End Test" ),
        new BuildTile() // "Ace Upgrade", "Ace Daily Upgrade" )
      };
    }

    public IEnumerable<Tile> Get() {
      // return Tiles;

      var tiles = Context.Tiles;
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

    }
  }
}
