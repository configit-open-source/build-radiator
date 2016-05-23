using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Configit.BuildRadiator.Helpers {
  public static class SignalRJsonSerializer {
    private static JsonSerializer _instance;

    public static JsonSerializer Instance {
      get {
        if ( _instance == null ) {
          var settings = new JsonSerializerSettings {
            ContractResolver = new SignalRContractResolver( new CamelCasePropertyNamesContractResolver() ),
            Converters = new JsonConverter[] {
              new StringEnumConverter { CamelCaseText = true }
            }
          };

          _instance = JsonSerializer.Create( settings );
        }

        return _instance;
      }
    }

    // SignalR types cannot be start with lowercase as the SignalR jQuery extension
    // assumes that keys start with Uppercase. 
    private class SignalRContractResolver: IContractResolver {
      private readonly IContractResolver _configitContractResolver;
      private readonly DefaultContractResolver _defaultContractResolver;

      public SignalRContractResolver( IContractResolver configitContractResolver ) {
        _configitContractResolver = configitContractResolver;
        _defaultContractResolver = new DefaultContractResolver();
      }

      public JsonContract ResolveContract( Type type ) {
        // only use resolver for "Configit" types (this may not be broad enough). 
        if ( type?.Namespace != null && type.Namespace.StartsWith( "Configit" ) ) {
          return _configitContractResolver.ResolveContract( type );
        }

        return _defaultContractResolver.ResolveContract( type );
      }
    }
  }
}