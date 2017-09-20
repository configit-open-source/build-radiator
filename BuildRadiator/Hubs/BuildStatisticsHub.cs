using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Configit.BuildRadiator.Helpers;
using Microsoft.AspNet.SignalR;

namespace Configit.BuildRadiator.Hubs {
  public class BuildStatisticsHub : Hub {
    private static readonly TimeSpan RefreshTimer = TimeSpan.FromHours( 1 );
    private static readonly BuildStatisticsService BuildStatisticsService;
    private string _branchName = "";
    private string _projectName = "";

    static BuildStatisticsHub() {
      var authHeader = EncodeBase64( ConfigurationManager.AppSettings["TeamCityUser"] + ":" + ConfigurationManager.AppSettings["TeamCityPassword"] );
      BuildStatisticsService = new BuildStatisticsService( ConfigurationManager.AppSettings["TeamCityUrl"], authHeader );

      var timer = new Timer( RefreshTimer.TotalMilliseconds );
      timer.Elapsed += TimerOnElapsed;
      timer.Start();
    }

    private static void TimerOnElapsed( object sender, ElapsedEventArgs e ) {
      UpdateClients();
    }

    private static void UpdateClients() {
      var context = GlobalHost.ConnectionManager.GetHubContext<BuildStatisticsHub>();
      Task.Run( async () => {
        var statistics = await BuildStatisticsService.Get( "Ace Commit" );
        context.Clients.All.Update( statistics );
      } );
    }

    public void Register( string projectName, string branchName ) {
      _projectName = projectName;
      _branchName = branchName;

      /*
      var groupName = BuildGroupName( projectName, branchName );
      Groups.Add( Context.ConnectionId, groupName );

      var groupTuple = Tuple.Create( projectName, branchName );
      GroupCounts.AddOrUpdate( groupTuple, key => 1, ( key, value ) => value + 1 );
      RefreshProject( groupTuple, true );
      */

      UpdateClients();
    }
    
    private static string EncodeBase64( string value ) {
      var byteArray = value.ToCharArray().Select( c => (byte)c );
      return Convert.ToBase64String( byteArray.ToArray() );
    }
  }
}