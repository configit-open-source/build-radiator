using System;
using System.Collections.Generic;
using Configit.BuildRadiator.Helpers;
using Configit.BuildRadiator.Model;
using Microsoft.AspNet.SignalR;

namespace Configit.BuildRadiator.Hubs {
  public class TileLayoutHub: Hub<ITileLayoutHubClient> {
    private static readonly TileService TileService;

    static TileLayoutHub() {
      TileService = new TileService();
      TileService.TileLayoutChanged += TileServiceOnTileLayoutChanged;
    }

    private static void TileServiceOnTileLayoutChanged( object sender, LayoutEventArgs e ) {
      Update( e.LayoutName );
    }

    public IReadOnlyList<Tile> Get( string layoutName ) {
      return TileService.Get( layoutName );
    }

    internal static void Update( string layoutName ) {
      var context = GlobalHost.ConnectionManager.GetHubContext<TileLayoutHub, ITileLayoutHubClient>();
      context.Clients.All.Update( layoutName );
    }
  }
}