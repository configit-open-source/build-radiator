using System;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Mvc;
using System.Web.Routing;
using Configit.BuildRadiator.Helpers;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using IAuthorizationFilter = System.Web.Mvc.IAuthorizationFilter;
using IDependencyResolver = Microsoft.AspNet.SignalR.IDependencyResolver;

namespace Configit.BuildRadiator {
  public class Global: HttpApplication {

    protected void Application_Start( object sender, EventArgs e ) {
      var authentication = new TeamCityAuthentication();

      ConfigureWebApi( GlobalConfiguration.Configuration, authentication );
      ConfigureMvc( GlobalFilters.Filters, authentication );
      ConfigureSignalR( GlobalHost.DependencyResolver );
      ConfigureRoutes( RouteTable.Routes );
    }

    private void ConfigureSignalR( IDependencyResolver configuration ) {
      configuration.Register( typeof( JsonSerializer ), () => SignalRJsonSerializer.Instance );
    }

    private static void ConfigureMvc( GlobalFilterCollection filters, IAuthorizationFilter authenticationFilter ) {
      filters.Add( authenticationFilter );
    }

    private static void ConfigureWebApi( HttpConfiguration configuration, IFilter authenticationFilter ) {
      // Remove WebApi XML serialization
      configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();

      // Enable custom authentication
      configuration.Filters.Add( authenticationFilter );

      // Configure Json Serializer
      configuration.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        Converters = new JsonConverter[] {
          new StringEnumConverter { CamelCaseText = true }
        }
      };
    }

    public static void ConfigureRoutes( RouteCollection routes ) {
      routes.IgnoreRoute( "{resource}.axd/{*pathInfo}" );
      routes.IgnoreRoute( "{resource}.ashx/{*pathInfo}" );

      // Don't route anything in the Content, Client or Bundles folder
      routes.Ignore( "Content/{*anything}" );
      routes.Ignore( "Client/{*anything}" );
      routes.Ignore( "bundles/{*anything}" );

      // Web Api Routes
      routes.MapHttpRoute(
        name: "DefaultApi",
        routeTemplate: "api/{controller}/{id}",
        defaults: new { id = RouteParameter.Optional }
      );

      // MVC Routes
      routes.MapRoute(
        name: "Default",
        url: "{controller}/{action}/{id}",
        defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
      );
    }
  }
}