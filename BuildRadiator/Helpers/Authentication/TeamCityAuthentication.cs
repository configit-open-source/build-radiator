using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Mvc;
using System.Web.Security;

namespace Configit.BuildRadiator.Helpers {
  internal class TeamCityAuthentication: AuthorizeAttribute, IAuthenticationFilter {
    internal static async Task<bool> Login( string username, string password ) {
      try {
        if ( !string.Equals( username, ConfigurationManager.AppSettings["TeamCityUser"], StringComparison.InvariantCultureIgnoreCase ) ) {
          return false;
        }

        if ( !string.Equals( password, ConfigurationManager.AppSettings["TeamCityPassword"], StringComparison.InvariantCulture ) ) {
          return false;
        }

        FormsAuthentication.SetAuthCookie( username, true );

        return true;
      } catch {
        return false;
      }
    }

    internal static void Logout() {
      FormsAuthentication.SignOut();
    }

    #region MVC
    protected override bool AuthorizeCore( HttpContextBase httpContext ) {
      var request = httpContext.Request;

      var cookie = request?.Cookies[FormsAuthentication.FormsCookieName];
      if ( cookie == null ) {
        return false;
      }

      var user = ValidateUser( cookie.Value );
      if ( user == null ) {
        return false;
      }

      httpContext.User = user;

      return true;
    }

    protected override void HandleUnauthorizedRequest( AuthorizationContext filterContext ) {
      var request = filterContext.HttpContext.Request;

      if ( request != null ) {
        var acceptsJson = request.AcceptTypes != null &&
                          request.AcceptTypes.Contains( "application/json", StringComparer.InvariantCultureIgnoreCase );

        if ( request.IsAjaxRequest() || acceptsJson ) {
          filterContext.Result = new HttpContentAndStatusResult {
            StatusCode = (int)HttpStatusCode.Unauthorized,
            StatusDescription = "Unauthorized",
            ContentType = "application/json",
            Content = @"{ ""errors"": [ { ""message"": ""Invalid login credentials or session timed out."", ""errorCode"": ""SESSION-TIMEOUT"" } ] }"
          };

          return;
        }
      }

      var returnUrl = request?.Url == null ? null : Uri.EscapeDataString( request.Url.PathAndQuery );
      var returnUrlQueryString = returnUrl == null ? null : "?return=" + returnUrl;

      filterContext.Result = new RedirectResult( "~/Login/" + returnUrlQueryString );
    }
    #endregion

    #region WebApi
    public Task AuthenticateAsync( HttpAuthenticationContext context, CancellationToken cancellationToken ) {
      var request = context.Request;

      if ( request == null ) {
        // If no http request then deny access as no checks can be made
        return Task.FromResult( 0 );
      }

      var user = ValidateUser( request );
      if ( user == null ) {
        return Task.FromResult( 0 );
      }

      context.Principal = user;

      return Task.FromResult( 0 );
    }

    private static IPrincipal ValidateUser( HttpRequestMessage request ) {
      var cookieContainer = request.Headers.GetCookies( FormsAuthentication.FormsCookieName ).FirstOrDefault();

      var cookie = cookieContainer?.Cookies.FirstOrDefault( c => c.Name == FormsAuthentication.FormsCookieName );
      if ( cookie == null ) {
        return null;
      }

      return ValidateUser( cookie.Value );
    }

    public Task ChallengeAsync( HttpAuthenticationChallengeContext context, CancellationToken cancellationToken ) {
      return Task.FromResult( 0 );
    }
    #endregion

    private static IPrincipal ValidateUser( string cookieValue ) {
      FormsAuthenticationTicket authenticationTicket;
      try {
        authenticationTicket = FormsAuthentication.Decrypt( cookieValue );
      } catch {
        return null;
      }

      if ( authenticationTicket == null ) {
        return null;
      }

      if ( authenticationTicket.Expired ) {
        return null;
      }

      var usernameInRequest = authenticationTicket.Name;

      // Update the cookie to extend the expiration time
      FormsAuthentication.SetAuthCookie( usernameInRequest, true );

      var identity = new GenericIdentity( usernameInRequest, "TeamCity" );
      var principal = new GenericPrincipal( identity, new string[0] );

      return principal;
    }
  }
}