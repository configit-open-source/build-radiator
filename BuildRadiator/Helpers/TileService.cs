using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Hosting;
using Configit.BuildRadiator.Model;

namespace Configit.BuildRadiator.Helpers {
  internal class TileService: IDisposable {
    private readonly string _tileLayoutFilename;
    private readonly FileSystemWatcher _fileSystemWatcher;
    private readonly TileLayoutReader _tileLayoutReader;

    private IReadOnlyList<Tile> _tiles;
    
    public event EventHandler TileLayoutChanged;

    public TileService() {
      _tileLayoutFilename = HostingEnvironment.MapPath( "~/App_Data/TileLayout/default.json" );
      _tileLayoutReader = new TileLayoutReader();

      _fileSystemWatcher = new FileSystemWatcher( Path.GetDirectoryName( _tileLayoutFilename ), Path.GetFileName( _tileLayoutFilename ) );
      _fileSystemWatcher.Changed += TileLayoutFileOnChanged;
      _fileSystemWatcher.EnableRaisingEvents = true;

      LoadTiles();
    }

    private async void TileLayoutFileOnChanged( object sender, FileSystemEventArgs e ) {
      await Task.Delay( 100 );

      LoadTiles();

      TileLayoutChanged?.Invoke( this, EventArgs.Empty );
    }

    private void LoadTiles() {
      _tiles = _tileLayoutReader.Deserialize( _tileLayoutFilename );
    }

    public IReadOnlyList<Tile> Get() {
      return _tiles;
    }

    public void Dispose() {
      _fileSystemWatcher.EnableRaisingEvents = false;
      _fileSystemWatcher.Dispose();
    }
  }
}