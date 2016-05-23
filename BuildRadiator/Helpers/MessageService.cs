using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Configit.BuildRadiator.Model;

namespace Configit.BuildRadiator.Helpers {
  public class MessageService {
    private static readonly IDictionary<string, Message> Messages;

    static MessageService() {
      var standardMessage = new[] { "standard" };

      Messages = new[] {
        new Message( "lastRelease", BuildMessage( "2.4.2", "Xenon Patch 2", "07 Jan 2016" ), standardMessage ),
        new Message( "sprintTheme", BuildTheme( "Logging &amp; Traceability", "Provide detailed information of changes within Ace to allow investigations into what happened and when"), "fancy" )
      }.ToDictionary( m => m.Key );
    }
    
    public Message Get( string messageKey ) {
      if ( Messages.ContainsKey( messageKey ) ) {
        return Messages[messageKey];
      }

      throw new ArgumentOutOfRangeException( nameof( messageKey ), "Message not found" );
    }
    
    private static string BuildTheme( string title, string summary ) {
      var sb = new StringBuilder();

      sb.Append( "<div class=\"title\">" );
      sb.Append( title );
      sb.Append( "</div>" );

      sb.Append( "<div class=\"quote\">" );
      sb.Append( summary );
      sb.Append( "</div>" );

      return sb.ToString();
    }

    private static string BuildMessage( string title, params string[] messages ) {
      var sb = new StringBuilder();

      sb.Append( "<div>" );
      sb.Append( title );
      sb.Append( "</div>" );

      if ( messages != null ) {
        foreach ( var message in messages ) {
          sb.Append( "<div class=\"small\">" );
          sb.Append( message );
          sb.Append( "</div>" );
        }
      }

      return sb.ToString();
    }
  }
}