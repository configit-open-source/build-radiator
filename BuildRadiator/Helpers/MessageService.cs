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
        new Message( "currentRelease", BuildMessage( "v5.0", "Bowser", "23 Apr 2018" ), standardMessage ),
        new Message( "sprintTheme", BuildTheme( "Numerics &amp; Release", "Adding numeric &amp; string support and improving Release Management" ), "fancy" )
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
