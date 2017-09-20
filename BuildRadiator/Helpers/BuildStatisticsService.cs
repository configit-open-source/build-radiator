using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml;
using Configit.BuildRadiator.Model.Builds;

namespace Configit.BuildRadiator.Helpers {
  public class BuildStatisticsService {
    public const string DefaultBranchName = "<default>";

    private readonly string _baseUrl;
    private readonly string _authenticationHeader;

    private static string _lastCheckDate = string.Empty;
    private static List<BuildStatistics> _allBuildStatistics;

    public BuildStatisticsService( string baseUrl, string authenticationHeader ) {
      _baseUrl = baseUrl.TrimEnd( '/' );
      _authenticationHeader = authenticationHeader;
      _allBuildStatistics = new List<BuildStatistics>();
    }

    private HttpClient CreateClient() {
      var client = new HttpClient {
        BaseAddress = new Uri( $"{_baseUrl}/httpAuth/app/rest/builds/id:151762/" ) // latest
      };

      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue( "Basic", _authenticationHeader );

      return client;
    }

    public async Task<BuildDetails> Get( string buildType, string branchName = DefaultBranchName ) {
      var client = CreateClient();

      var checkDate = DateTime.Now.ToShortDateString();

      if ( checkDate != _lastCheckDate ) {
        var buildStatisticsTask = Task.Run( () => GetStatistics( client, buildType, branchName ) );

        await Task.WhenAll( buildStatisticsTask );

        var parser = new BuildStatisticsParser( buildStatisticsTask.Result );

        var buildStatistics = parser.Parse();

        _allBuildStatistics.Add( buildStatistics );
        if ( _allBuildStatistics.Count > 5 ) {
          _allBuildStatistics.RemoveAt( 0 );
        }

        _lastCheckDate = checkDate;
      }
      
      return new BuildDetails( buildType, branchName, _allBuildStatistics );
    }

    private static async Task<XmlDocument> GetStatistics( HttpClient client, string buildType, string branchName ) {
      var url = "statistics";//"/buildType:name:" + Uri.EscapeDataString( buildType ) + ",running:any,branch:" + Uri.EscapeUriString( branchName ) + ",count:1?fields=defaultBranch,buildType,branchName,status,statusText,startDate,finishDate,running,running-info";
      var cacheBuster = "?t=" + DateTime.UtcNow.Ticks;

      var data = await client.GetStringAsync( url + cacheBuster );

      var document = new XmlDocument();
      document.LoadXml( data );

      return document;
    }

    public class BuildDetails {
      public string Name { get; }
      public string BranchName { get; }
      public List<BuildStatistics> Statistics { get; }

      public BuildDetails( string name, string branchName, List<BuildStatistics> statistics) {
        Name = name;
        BranchName = branchName;
        Statistics = statistics;
      }
    }
  }
}