'use strict';

angular.module( 'BuildRadiator', ['ngMaterial', 'ngMessages', 'ngynServerConnection', 'ngynSignalRServerConnectionBackend'] );

angular.module( 'BuildRadiator' ).config( ['$httpProvider', '$provide', ( $httpProvider, $provide ) => {

  $provide.factory( 'httpInterceptor', ['$window', '$location', '$q', ( $window, $location, $q ) => ( {
    responseError: response => {
      if ( response.status === 401 ) {
        $window.location = 'Login?return=' + encodeURIComponent( $location.absUrl() );
      }
      return $q.reject( response );
    }
  } )] );

  $httpProvider.interceptors.push( 'httpInterceptor' );
}] );