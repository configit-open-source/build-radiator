( function() {
  'use strict';

  var module = angular.module( 'BuildRadiator' );

  module.service( 'MessageHub', ['ServerConnection', function( ServerConnection ) {
    return ServerConnection( 'messageHub' );
  }] );
} )();