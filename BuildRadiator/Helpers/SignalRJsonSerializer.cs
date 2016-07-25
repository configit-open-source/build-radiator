using System;
using System.Reflection;
using Microsoft.AspNet.SignalR.Infrastructure;
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
      private readonly IContractResolver _contractResolver;
      private readonly DefaultContractResolver _defaultContractResolver;
      private readonly Assembly _signalrAssembly;

      public SignalRContractResolver( IContractResolver contractResolver ) {
        _contractResolver = contractResolver;
        _defaultContractResolver = new DefaultContractResolver();

        _signalrAssembly = typeof( Connection ).Assembly;
      }

      public JsonContract ResolveContract( Type type ) {
        if ( type == null || type.Assembly == _signalrAssembly ) {
          return _defaultContractResolver.ResolveContract( type );
        }

        return _contractResolver.ResolveContract( type );
      }
    }
  }
}