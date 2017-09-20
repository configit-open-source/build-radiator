( function() {
  'use strict';

  var module = angular.module( 'BuildRadiator' );

  module.service( 'BuildStatisticsHub', ['ServerConnection', function( ServerConnection ) {
    return ServerConnection( 'buildStatisticsHub' );
  }] );
} )();