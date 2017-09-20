using System;

namespace Configit.BuildRadiator.Model.Builds {
  public class BuildStatistics {
    public double ClassCodeCoverage { get; }
    public double MethodCodeCoverage { get; }
    public double StatementCodeCoverage { get; }

    public BuildStatistics( double classCodeCoverage, double methodCodeCoverage, double statementCodeCoverage ) {
      ClassCodeCoverage = classCodeCoverage;
      MethodCodeCoverage = methodCodeCoverage;

      var r = new Random();
      StatementCodeCoverage = Math.Round( statementCodeCoverage - 5 + r.NextDouble() * 10 , 1 );
    }
  }
}