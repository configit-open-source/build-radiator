using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml;

using Configit.BuildRadiator.Model.Builds;

namespace Configit.BuildRadiator.Helpers {
  public class BuildService {
    public const string DefaultBranchName = "<default>";

    private readonly string _baseUrl;
    private readonly string _authenticationHeader;

    public BuildService( string baseUrl, string authenticationHeader ) {
      _baseUrl = baseUrl.TrimEnd( '/' );
      _authenticationHeader = authenticationHeader;
    }

    private HttpClient CreateClient() {
      var client = new HttpClient {
        BaseAddress = new Uri( $"{_baseUrl}/httpAuth/app/rest/latest/" )
      };

      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue( "Basic", _authenticationHeader );

      return client;
    }

    public async Task<Build> Get( string buildId, string branchName ) {
      var client = CreateClient();

      var buildInfoTask = Task.Run( () => GetBuildInfo( client, buildId, branchName ) );
      var investigationInfoTask = Task.Run( () => GetInvestigationInfo( client, buildId ) );
      var changesSinceFailureInfoTask = Task.Run( () => GetChangesSinceFailureInfo( client, buildId, branchName ) );

      await Task.WhenAll( buildInfoTask, investigationInfoTask, changesSinceFailureInfoTask );

      var parser = new BuildParser( buildInfoTask.Result, investigationInfoTask.Result, changesSinceFailureInfoTask.Result );

      var build = parser.Parse();

      client.Dispose();

      return build;
    }

    private static async Task<XmlDocument> GetBuildInfo( HttpClient client, string buildId, string branchName ) {
      var branchQuery = !string.IsNullOrWhiteSpace( branchName ) ? $",branch:{Uri.EscapeUriString( branchName )}" : null;
      var url = $"builds/buildType:id:{Uri.EscapeDataString( buildId )},running:any{branchQuery},count:1?fields=defaultBranch,buildType,branchName,status,statusText,startDate,finishDate,running,running-info";
      var cacheBuster = $"&t={DateTime.UtcNow.Ticks}";

      var data = await client.GetStringAsync( url + cacheBuster );

      var document = new XmlDocument();
      document.LoadXml( data );

      return document;
    }

    private static async Task<XmlDocument> GetInvestigationInfo( HttpClient client, string buildId ) {
      var url = $"investigations?locator=buildType:id:{Uri.EscapeDataString( buildId )}&fields=investigation(state,assignee(name,email),assignment(text,user(name,email)))";
      var cacheBuster = $"&t={DateTime.UtcNow.Ticks}";

      var data = await client.GetStringAsync( url + cacheBuster );

      var document = new XmlDocument();
      document.LoadXml( data );

      return document;
    }

    private static async Task<XmlDocument> GetChangesSinceFailureInfo( HttpClient client, string buildId, string branchName ) {
      var branchQuery = !string.IsNullOrWhiteSpace( branchName ) ? $",branch:{Uri.EscapeUriString( branchName )}" : null;
      var buildLocator = $"buildType:id:{Uri.EscapeDataString( buildId )}{branchQuery}";
      var url = $"builds/?locator={buildLocator},status:failure,running:false,sinceBuild:({buildLocator},status:success,running:false)&fields=build(changes(change(username,user(email))))";
      var cacheBuster = $"&t={DateTime.UtcNow.Ticks}";

      var document = new XmlDocument();
      try {
        var data = await client.GetStringAsync( url + cacheBuster );
        document.LoadXml( data );
      }
      catch {
        Debug.WriteLine( "Could not load changes, probably because there are no successful builds" );
      }
      return document;

    }
  }
}