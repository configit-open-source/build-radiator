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
        new Message( "lastRelease", BuildMessage( "3.0", "Carbon", "17 Oct 2016" ), standardMessage ),
        new Message( "sprintTheme", BuildTheme( "Product Split", "Separating Delivery and Product code" ), "fancy" )
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
