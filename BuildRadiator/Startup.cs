using Configit.BuildRadiator;

using Microsoft.Owin;

using Owin;

[assembly: OwinStartup( typeof( Startup ) )]

namespace Configit.BuildRadiator {
  public class Startup {
    public void Configuration( IAppBuilder app ) {
      app.MapSignalR();
    }
  }
}