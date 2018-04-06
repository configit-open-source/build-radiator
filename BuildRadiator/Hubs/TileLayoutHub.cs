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

    private static void TileServiceOnTileLayoutChanged( object sender, EventArgs eventArgs ) {
      Update();
    }

    public IReadOnlyList<Tile> Get() {
      return TileService.Get();
    }

    internal static void Update() {
      var context = GlobalHost.ConnectionManager.GetHubContext<TileLayoutHub>();
      context.Clients.All.Update();
    }
  }
}