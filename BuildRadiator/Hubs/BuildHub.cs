using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Timers;
using Configit.BuildRadiator.Helpers;
using Configit.BuildRadiator.Model.Builds;
using Microsoft.AspNet.SignalR;

namespace Configit.BuildRadiator.Hubs {
  public class BuildHub: Hub<IBuildHubClient> {
    private static readonly TimeSpan RefreshTimer = TimeSpan.FromSeconds( 10 );
    private static readonly ConcurrentDictionary<Tuple<string, string>, int> GroupCounts;
    private static readonly ConcurrentDictionary<Tuple<string, string>, Build> PreviousBuild;
    private static readonly BuildService BuildService;
    private static readonly ServiceController _teamCityService;

    static BuildHub() {
      GroupCounts = new ConcurrentDictionary<Tuple<string, string>, int>();
      PreviousBuild = new ConcurrentDictionary<Tuple<string, string>, Build>();

      var tcServiceName = ConfigurationManager.AppSettings["TeamCityService"];
      if ( !string.IsNullOrEmpty( tcServiceName ) ) {
        try {
          _teamCityService = new ServiceController( tcServiceName );
        }
        catch {
          // Ignore if we can't find the servie
        }
      }

      var authHeader = EncodeBase64( ConfigurationManager.AppSettings["TeamCityUser"] + ":" + ConfigurationManager.AppSettings["TeamCityPassword"] );
      BuildService = new BuildService( ConfigurationManager.AppSettings["TeamCityUrl"], authHeader );

      var timer = new Timer( RefreshTimer.TotalMilliseconds );
      timer.Elapsed += TimerOnElapsed;
      timer.Start();
    }

    private static void TimerOnElapsed( object sender, ElapsedEventArgs e ) {
      foreach ( var project in GroupCounts.Keys ) {
        int count;
        if ( GroupCounts.TryGetValue( project, out count ) && count > 0 ) {
          RefreshProject( project );
        }
      }

      if ( _teamCityService != null ) {
        _teamCityService.Refresh();
        var context = GlobalHost.ConnectionManager.GetHubContext<BuildHub>();
        context.Clients.All.ServiceStatus( _teamCityService.Status );
      }
    }

    public void Register( string projectName, string branchName ) {
      var groupName = BuildGroupName( projectName, branchName );
      Groups.Add( Context.ConnectionId, groupName );

      var groupTuple = Tuple.Create( projectName, branchName );
      GroupCounts.AddOrUpdate( groupTuple, key => 1, ( key, value ) => value + 1 );
      RefreshProject( groupTuple, true );
    }

    public void Unregister( string projectName, string branchName ) {
      var groupName = BuildGroupName( projectName, branchName );
      Groups.Remove( Context.ConnectionId, groupName );

      var groupTuple = Tuple.Create( projectName, branchName );
      GroupCounts.AddOrUpdate( groupTuple, key => 0, ( key, value ) => value - 1 );
    }

    private static void RefreshProject( Tuple<string, string> project, bool forceRefresh = false ) {
      Build previousBuild;
      PreviousBuild.TryGetValue( project, out previousBuild );

      Task.Run( async () => {
        try {
          var build = await BuildService.Get( project.Item1, project.Item2 );
          if ( !forceRefresh && BuildComparer.AreIdentical( build, previousBuild ) ) {
            return;
          }

          PreviousBuild.AddOrUpdate( project, build, ( k, v ) => build );
          Update( build );
        }
        catch ( Exception ex ) {
          var buildError = new BuildError {
            Name = project.Item1,
            BranchName = project.Item2,
            Error = ex
          };

          PreviousBuild.TryRemove( project, out previousBuild );
          UpdateError( buildError );
        }
      } );
    }

    internal static void Update( Build build ) {
      var groupName = BuildGroupName( build.Name, build.BranchName );
      var context = GlobalHost.ConnectionManager.GetHubContext<BuildHub>();
      context.Clients.Group( groupName ).Update( build );
    }

    internal static void UpdateError( BuildError build ) {
      var groupName = BuildGroupName( build.Name, build.BranchName );
      var context = GlobalHost.ConnectionManager.GetHubContext<BuildHub>();
      context.Clients.Group( groupName ).UpdateError( build );
    }

    private static string BuildGroupName( string projectName, string branchName ) {
      return $"Build${projectName}${branchName}";
    }

    private static string EncodeBase64( string value ) {
      var byteArray = value.ToCharArray().Select( c => (byte) c );
      return Convert.ToBase64String( byteArray.ToArray() );
    }
  }

  internal class BuildError {
    public string Name { get; set; }

    public string BranchName { get; set; }

    public Exception Error { get; set; }
  }
}