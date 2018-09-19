﻿using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using Configit.BuildRadiator.Model.Builds;

namespace Configit.BuildRadiator.Helpers {
  public class BuildParser {
    private static readonly Regex UsernameRegEx = new Regex( "<([^>]*)>$", RegexOptions.Compiled );

    private readonly XmlNode _buildInfo;
    private readonly XmlNodeList _investigationInfo;
    private readonly XmlNodeList _changesSinceLastFailure;
    private readonly bool _noChangesInfo;

    public BuildParser( XmlNode buildInfo, XmlNode investigationInfo, XmlDocument changesSinceFailureInfo ) {
      if ( buildInfo == null ) {
        throw new ArgumentNullException( nameof( buildInfo ) );
      }
      if ( investigationInfo == null ) {
        throw new ArgumentNullException( nameof( investigationInfo ) );
      }
      if ( changesSinceFailureInfo == null ) {
        throw new ArgumentNullException( nameof( changesSinceFailureInfo ) );
      }

      _buildInfo = buildInfo.SelectSingleNode( "./build" );
      _investigationInfo = investigationInfo.SelectNodes( "./investigations/investigation" );
      _changesSinceLastFailure = changesSinceFailureInfo.SelectNodes( "./builds/build/changes/change" );

      _noChangesInfo = string.IsNullOrEmpty( changesSinceFailureInfo.InnerXml );

      if ( _buildInfo == null ) {
        throw new Exception( "Invalid xml" );
      }
      if ( _investigationInfo == null ) {
        throw new Exception( "Invalid xml" );
      }
      if ( _changesSinceLastFailure == null ) {
        throw new Exception( "Invalid xml" );
      }
    }

    public Build Parse() {
      var isRunning = bool.Parse( _buildInfo.GetInnerText( "./@running", "false" ) );

      var isDefaultBranch = bool.Parse( _buildInfo.GetInnerText( "./@defaultBranch", "false" ) );
      var branchName = isDefaultBranch ? BuildService.DefaultBranchName : _buildInfo.GetInnerText( "./@branchName" );

      var build = new Build {
        Id = _buildInfo.GetInnerText( "./buildType/@id" ),
        Name = _buildInfo.GetInnerText( "./buildType/@name" ),
        Url = BuildUrl( _buildInfo.GetInnerText( "./buildType/@webUrl" ), branchName ),
        BranchName = branchName,
        Status = isRunning ? BuildStatus.InProgress : ParseStatus( _buildInfo.GetInnerText( "./@status" ) ),
        StatusText = _buildInfo.GetInnerText( "./statusText" ),
        Start = ParseDate( _buildInfo.GetInnerText( "./startDate" ) ),
        End = GetEndDate( _buildInfo, isRunning ),
        PercentComplete = isRunning ? int.Parse( _buildInfo.GetInnerText( "./running-info/@percentageComplete", "0" ) ) : 100,
        StatusSubText = isRunning ? _buildInfo.GetInnerText( "./running-info/@currentStageText" ) : null
      };

      if ( _investigationInfo.Count > 0 ) {
        var investigation = _investigationInfo[0];
        var state = investigation.GetInnerText( "./@state" );
        if ( state != "GIVEN_UP" ) {
          build.Investigator = investigation.GetInnerText( "./assignee/@email" );
        }
      }

      if ( build.Status != BuildStatus.Failed ) {
        if ( isRunning ) {
          build.PreviouslyFailing = _changesSinceLastFailure.Count > 0 || _noChangesInfo;
        }
        return build;
      }

      foreach ( XmlNode change in _changesSinceLastFailure ) {
        var username = change.GetInnerText( "./user/@email" );
        if ( string.IsNullOrWhiteSpace( username ) ) {
          username = ParseUsername( change.GetInnerText( "./@username" ) );
        }

        if ( !string.IsNullOrWhiteSpace( username ) && !build.Committers.Contains( username ) ) {
          build.Committers.Add( username );
        }
      }

      return build;
    }

    private static string BuildUrl( string baseUrl, string branchName ) {
      var delimeter = baseUrl.Contains( "?" ) ? "&" : "?";

      return $"{baseUrl}{delimeter}branch={Uri.EscapeDataString( branchName )}";
    }

    private static DateTime GetEndDate( XmlNode buildInfo, bool isRunning ) {
      if ( isRunning ) {
        var estimatedTotalSeconds = int.Parse( buildInfo.GetInnerText( "./running-info/@estimatedTotalSeconds" ) );
        var elapsedSeconds = int.Parse( buildInfo.GetInnerText( "./running-info/@elapsedSeconds" ) );

        var endTime = DateTime.UtcNow.AddSeconds( estimatedTotalSeconds - elapsedSeconds );
        return endTime.ToSecondPrecision();
      }

      return ParseDate( buildInfo.GetInnerText( "./finishDate" ) );
    }

    private static BuildStatus ParseStatus( string status ) {
      switch ( status ) {
        case "SUCCESS":
          return BuildStatus.Success;
        case "FAILURE":
          return BuildStatus.Failed;
        default:
          return BuildStatus.Unknown;
      }
    }

    private static DateTime ParseDate( string dateString ) {
      return DateTime.ParseExact( dateString, "yyyyMMddTHHmmsszzz", CultureInfo.InvariantCulture ).ToSecondPrecision();
    }

    private static string ParseUsername( string username ) {
      var match = UsernameRegEx.Match( username );
      return match.Success ? match.Groups[1].Value : null;
    }
  }
}