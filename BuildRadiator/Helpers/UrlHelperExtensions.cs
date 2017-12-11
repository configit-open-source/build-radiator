using System;
using System.IO;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Configit.BuildRadiator.Helpers {
  public static class UrlHelperExtensions {
    private static readonly DateTime StartTime = new DateTime( 2017, 1, 1 );

    public static string MinifiedContent( this UrlHelper urlHelper, string contentPath ) {
      var suffix = GetSuffix( contentPath );

      if ( urlHelper.RequestContext.HttpContext.IsDebuggingEnabled ) {
        // Do not add .min to path if in debug mode
        return urlHelper.Content( contentPath ) + suffix;
      }

      if ( string.IsNullOrWhiteSpace( contentPath ) ) {
        return contentPath;
      }

      var lastDotIndex = contentPath.LastIndexOf( '.' );
      if ( lastDotIndex < 0 ) {
        return contentPath;
      }

      var extension = contentPath.Substring( lastDotIndex );
      var newContentPath = contentPath.Substring( 0, lastDotIndex ) + ".min" + extension;
      
      return urlHelper.Content( newContentPath ) + suffix;
    }

    private static string GetSuffix( string contentPath ) {
      var filename = HostingEnvironment.MapPath( contentPath );

      if ( filename == null ) {
        return null;
      }

      var fileInfo = new FileInfo( filename );
      if ( !fileInfo.Exists ) {
        return null;
      }

      var time = (int)( fileInfo.LastWriteTimeUtc - StartTime ).TotalSeconds;

      return $"?t={time}";
    }
  }
}