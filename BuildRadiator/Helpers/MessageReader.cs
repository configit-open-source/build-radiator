using System.Collections.Generic;
using System.IO;
using System.Linq;
using Configit.BuildRadiator.Model;
using Newtonsoft.Json.Linq;

namespace Configit.BuildRadiator.Helpers {
  internal class MessageReader {
    public IDictionary<string, Message> Deserialize( string messageFile ) {
      var messages = new List<Message>();

      var messageJson = JObject.Parse( File.ReadAllText( messageFile ) );
      foreach ( var messageProperty in messageJson.Properties() ) {
        var message = DeserializeMessage( messageProperty );

        messages.Add( message );
      }

      return messages.ToDictionary( m => m.Key );
    }

    private static Message DeserializeMessage( JProperty messageProperty ) {
      var messageKey = messageProperty.Name;
      var messageConfig = messageProperty.Value;

      var content = messageConfig.Value<string>( "content" );
      var classes = messageConfig.Value<JArray>( "classes" ).Values<string>();

      return new Message( messageKey, content, classes );
    }
  }
}