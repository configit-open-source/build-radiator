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

    private readonly RadiatorContext _context;

    private void InitializeDatabase() {
      Database.SetInitializer( new DropCreateDatabaseAlways<RadiatorContext>() );

      try {
        _context.Database.Initialize( true );

        var clockTile = new ClockTile() { Order = 0, Title = "Otter Office", TimeZoneId = "Europe/London" };
        var clockTile2 = new ClockTile() { Order = 7, Title = "Denmark Office", TimeZoneId = "Europe/Copenhagen" };
        var clockTile3 = new ClockTile() { Order = 8, Title = "US Office", TimeZoneId = "America/New_York" };

        var message = new Message() { Key = "Otter", Content = "<img src='https://media.giphy.com/media/l0K45p4XQTVmyClMs/giphy-downsized-large.gif' />" };
        var message2 = new Message() { Key = "Log", Content = "<h1 style=\"color: orange;Height:200px ;background-image: url('http://p.fod4.com/p/media/413ade39e1/sCHdfjwxRui3uyB9h8mr_o3.gif'\">Otters rock!</H1>" };
        var message3 = new Message() { Key = "Hamster", Content = "<img src='https://68.media.tumblr.com/56ab2b83f9308cb58b5deb82c4c53cfd/tumblr_oie73yaAfM1rjlj53o1_500.gif' />" };
        _context.Messages.Add( message );
        _context.Messages.Add( message2 );
        _context.Messages.Add( message3 );

        var messageTile = new MessageTile() { Order = 1, Title = "Important otter stuff", MessageKey = "Otter", ColumnSpan = 2 };
        var messageTile2 = new MessageTile() { Order = 3, Title = "Ottercam", MessageKey = "Log", ColumnSpan = 2 };

        var server = new BuildServer() { Password = "2vov3rap", Url = "http://build.configit.com", User = "Bob" };
        _context.BuildServers.Add( server );

        var build = new Build {
          BranchName = "selene",
          Name = "Ace Commit",
          Server = server
        };

        var build2 = new Build {
          BranchName = "master",
          Name = "Ace Daily Deploy",
          Server = server
        };

        var buildTile = new BuildTile() { Order = 2, Title = "Build them all", Build = build };
        var buildTile2 = new BuildTile() { Order = 4, Title = "Ace Otters", Build = build2 };

        var tiles = new List<Tile>() { buildTile, clockTile, messageTile, messageTile2, buildTile2, clockTile2, clockTile3 }; //, , clockTile2

        var page1 = new Page() { Title = "Page 1", Tiles = tiles };
        page1.Tiles = tiles;

        _context.Pages.Add( page1 );

        _context.SaveChanges();
      }
      catch ( DbEntityValidationException ex ) {
        Console.WriteLine( ex.Message );
      }
    }

    public TileController() {
      _context = new RadiatorContext();
      _context.Configuration.LazyLoadingEnabled = false;
      //InitializeDatabase();
      
    }


    public IEnumerable<Tile> Get() {

      var buildTiles = (IEnumerable<Tile>) _context.Tiles.OfType<BuildTile>().Include( "Build" );
      var messageTiles = (IEnumerable<Tile>) _context.Tiles.OfType<MessageTile>();
      var clockTiles = (IEnumerable<Tile>) _context.Tiles.OfType<ClockTile>();
      var tiles = buildTiles.Concat( messageTiles ).Concat( clockTiles );

      return tiles?.OrderBy( o => o.Order ).ToList();

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
