using System;
using System.Xml;
using Configit.BuildRadiator.Model.Builds;

namespace Configit.BuildRadiator.Helpers {
  public class BuildStatisticsParser {
    private readonly XmlNode _properties;

    public BuildStatisticsParser( XmlNode buildInfo ) {
      if ( buildInfo == null ) {
        throw new ArgumentNullException( nameof( buildInfo ) );
      }

      _properties = buildInfo.SelectSingleNode( "./properties" );

      if ( _properties == null ) {
        throw new Exception( "Invalid xml" );
      }
    }

    public BuildStatistics Parse() {
      var classCoverage = Math.Round( double.Parse( _properties.SelectSingleNode("property[@name='CodeCoverageC']").Attributes["value"].InnerText ), 1 );
      var methodCoverage = Math.Round( double.Parse( _properties.SelectSingleNode("property[@name='CodeCoverageM']").Attributes["value"].InnerText ), 1 );
      var statementCoverage = Math.Round( double.Parse( _properties.SelectSingleNode("property[@name='CodeCoverageS']").Attributes["value"].InnerText ), 1 );

      return new BuildStatistics( classCoverage, methodCoverage, statementCoverage );
    }
  }
}