using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Configit.BuildRadiator.Controllers;
using Configit.BuildRadiator.Model;

namespace Configit.BuildRadiator.Helpers {
  public class MessageService {
    // private static readonly IDictionary<string, Message> Messages;
    private static readonly TileController.RadiatorContext Context;

    static MessageService() {
      var standardMessage = new[] { "standard" };
      Context = new TileController.RadiatorContext();
    }

    public Message Get( string messageKey )
    {

     var message= Context.Messages.FirstOrDefault( m => m.Key == messageKey );

      return message;

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