﻿using System.Security.Principal;

namespace Configit.BuildRadiator.Helpers {
  public class TeamCityPrincipal: GenericPrincipal {
    public TeamCityPrincipal( IIdentity identity, string authenticationHeader, string[] roles )
      : base( identity, roles ) {
      AuthenticationHeader = authenticationHeader;
    }

    public string AuthenticationHeader { get; set; }
  }
}