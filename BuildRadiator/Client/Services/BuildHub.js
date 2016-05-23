( function() {
  'use strict';

  var module = angular.module( 'BuildRadiator' );

  module.service( 'BuildHub', ['ServerConnection', function( ServerConnection ) {
    return ServerConnection( 'buildHub' );
  }] );
} )();