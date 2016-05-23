using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Configit.BuildRadiator.Model.Builds;

namespace Configit.BuildRadiator.Helpers {
  public static class BuildComparer {
    public static bool AreIdentical( Build build1, Build build2 ) {
      if ( build1 == null && build2 == null ) {
        return true;
      }

      if ( build1 == null || build2 == null ) {
        return false;
      }

      if ( !string.Equals( build1.Name, build2.Name, StringComparison.InvariantCulture ) ) {
        return false;
      }

      if ( !string.Equals( build1.BranchName, build2.BranchName, StringComparison.InvariantCulture ) ) {
        return false;
      }

      if ( build1.Status != build2.Status ) {
        return false;
      }

      if ( !string.Equals( build1.StatusText, build2.StatusText, StringComparison.InvariantCulture ) ) {
        return false;
      }

      if ( !string.Equals( build1.StatusSubText, build2.StatusSubText, StringComparison.InvariantCulture ) ) {
        return false;
      }

      if ( build1.PreviouslyFailing != build2.PreviouslyFailing ) {
        return false;
      }

      if ( !build1.Start.Equals( build2.Start ) ) {
        return false;
      }

      if ( !build1.End.Equals( build2.End ) ) {
        return false;
      }

      if ( build1.PercentComplete != build2.PercentComplete ) {
        return false;
      }

      if ( !string.Equals( build1.Investigator, build2.Investigator, StringComparison.InvariantCulture ) ) {
        return false;
      }

      if ( build1.Committers.Count != build2.Committers.Count ) {
        return false;
      }

      if ( !build1.Committers.All( c => build2.Committers.Contains( c, StringComparer.InvariantCulture ) ) ) {
        return false;
      }

      return true;
    }
  }
}