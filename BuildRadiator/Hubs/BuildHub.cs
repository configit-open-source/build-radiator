using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Configit.BuildRadiator.Helpers;
using Configit.BuildRadiator.Model.Builds;
using Microsoft.AspNet.SignalR;

namespace Configit.BuildRadiator.Hubs {
  public class BuildHub: Hub<IBuildHubClient> {
    private static readonly TimeSpan RefreshTimer = TimeSpan.FromSeconds( 10 );
    private static readonly ConcurrentDictionary<(string buildId, string branchName), int> GroupCounts;
    private static readonly ConcurrentDictionary<(string buildId, string branchName), Build> PreviousBuild;
    private static readonly BuildService BuildService;

    static BuildHub() {
      GroupCounts = new ConcurrentDictionary<(string buildId, string branchName), int>();
      PreviousBuild = new ConcurrentDictionary<(string buildId, string branchName), Build>();

      var authHeader = EncodeBase64( ConfigurationManager.AppSettings["TeamCityUser"] + ":" + ConfigurationManager.AppSettings["TeamCityPassword"] );
      BuildService = new BuildService( ConfigurationManager.AppSettings["TeamCityUrl"], authHeader );

      var timer = new Timer( RefreshTimer.TotalMilliseconds );
      timer.Elapsed += TimerOnElapsed;
      timer.Start();
    }

    private static void TimerOnElapsed( object sender, ElapsedEventArgs e ) {
      foreach ( var project in GroupCounts.Keys ) {
        if ( GroupCounts.TryGetValue( project, out var count ) && count > 0 ) {
          RefreshProject( project );
        }
      }
    }

    public void Register( string buildId, string branchName ) {
      var groupName = BuildGroupName( buildId, branchName );
      Groups.Add( Context.ConnectionId, groupName );

      var groupTuple = (buildId, branchName);
      GroupCounts.AddOrUpdate( groupTuple, key => 1, ( key, value ) => value + 1 );
      RefreshProject( groupTuple, true );
    }

    public void Unregister( string buildId, string branchName ) {
      var groupName = BuildGroupName( buildId, branchName );
      Groups.Remove( Context.ConnectionId, groupName );

      var groupTuple = (buildId, branchName);
      GroupCounts.AddOrUpdate( groupTuple, key => 0, ( key, value ) => value - 1 );
    }

    private static void RefreshProject( (string buildId, string branchName) project, bool forceRefresh = false ) {
      PreviousBuild.TryGetValue( project, out var previousBuild );

      Task.Run( async () => {
        try {
          var build = await BuildService.Get( project.buildId, project.branchName );
          if ( !forceRefresh && BuildComparer.AreIdentical( build, previousBuild ) ) {
            return;
          }

          PreviousBuild.AddOrUpdate( project, build, ( k, v ) => build );
          Update( build );
        }
        catch ( Exception ex ) {
          var buildError = new BuildError {
            BuildId = project.buildId,
            BranchName = project.branchName,
            Error = ex
          };

          PreviousBuild.TryRemove( project, out previousBuild );
          UpdateError( buildError );
        }
      } );
    }

    internal static void Update( Build build ) {
      var groupName = BuildGroupName( build.Id, build.BranchName );
      var context = GlobalHost.ConnectionManager.GetHubContext<BuildHub, IBuildHubClient>();
      context.Clients.Group( groupName ).Update( build );
    }

    internal static void UpdateError( BuildError buildError ) {
      var groupName = BuildGroupName( buildError.BuildId, buildError.BranchName );
      var context = GlobalHost.ConnectionManager.GetHubContext<BuildHub, IBuildHubClient>();
      context.Clients.Group( groupName ).UpdateError( buildError );
    }

    private static string BuildGroupName( string buildId, string branchName ) {
      return $"Build${buildId}${branchName}";
    }

    private static string EncodeBase64( string value ) {
      var byteArray = value.ToCharArray().Select( c => (byte)c );
      return Convert.ToBase64String( byteArray.ToArray() );
    }
  }

  public class BuildError {
    public string BuildId { get; set; }

    public string BranchName { get; set; }

    public Exception Error { get; set; }
  }
}