using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Hosting;
using Configit.BuildRadiator.Model;

namespace Configit.BuildRadiator.Helpers {
  internal class TileService: IDisposable {
    private readonly string _tileLayoutDirectory;
    private readonly FileSystemWatcher _fileSystemWatcher;
    private readonly TileLayoutReader _tileLayoutReader;
    private readonly IDictionary<string, IReadOnlyList<Tile>> _tiles;

    public event EventHandler<LayoutEventArgs> TileLayoutChanged;

    public TileService() {
      _tiles = new Dictionary<string, IReadOnlyList<Tile>>( StringComparer.InvariantCultureIgnoreCase );
      _tileLayoutDirectory = HostingEnvironment.MapPath( "~/App_Data/TileLayout/" );
      _tileLayoutReader = new TileLayoutReader();

      _fileSystemWatcher = new FileSystemWatcher( _tileLayoutDirectory, "*.json" );
      _fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size;
      _fileSystemWatcher.Changed += TileLayoutFileOnChanged;
      _fileSystemWatcher.EnableRaisingEvents = true;

      LoadAllTiles();
    }

    private void LoadAllTiles() {
      foreach ( var layoutFilename in Directory.GetFiles( _tileLayoutDirectory, "*.json" ) ) {
        LoadTileLayout( layoutFilename );
      }
    }

    private async void TileLayoutFileOnChanged( object sender, FileSystemEventArgs e ) {
      await Task.Delay( 100 );

      var layoutName = LoadTileLayout( e.FullPath );

      TileLayoutChanged?.Invoke( this, new LayoutEventArgs( layoutName ) );
    }

    private string LoadTileLayout( string layoutFilename ) {
      var layoutName = Path.GetFileNameWithoutExtension( layoutFilename );

      _tiles[layoutName] = _tileLayoutReader.Deserialize( layoutFilename );

      return layoutName;
    }

    public IReadOnlyList<Tile> Get( string layoutName ) {
      if ( !_tiles.ContainsKey( layoutName ) ) {
        return new List<Tile>();
      }
      return _tiles[layoutName];
    }

    public void Dispose() {
      _fileSystemWatcher.EnableRaisingEvents = false;
      _fileSystemWatcher.Dispose();
    }
  }
}