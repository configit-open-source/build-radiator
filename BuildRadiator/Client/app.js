﻿( function() {
  'use strict';

  angular.module( 'BuildRadiator', ['ngMaterial', 'ngMessages', 'ngynServerConnection', 'ngynSignalRServerConnectionBackend'] );

  angular.module( 'BuildRadiator' ).config( ['$httpProvider', '$provide', function( $httpProvider, $provide ) {

    $provide.factory( 'httpInterceptor', ['$window', '$location', '$q', function( $window, $location, $q ) {
      return {
        responseError: function( response ) {
          if ( response.status === 401 ) {
            $window.location = 'Login?return=' + encodeURIComponent( $location.absUrl() );
          }
          return $q.reject( response );
        }
      };
    }] );

    $httpProvider.interceptors.push( 'httpInterceptor' );
  }] );
} )();