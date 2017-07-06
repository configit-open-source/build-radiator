( function() {
  'use strict';

  var module = angular.module( 'BuildRadiator' );

  module.directive( 'clock', ['$interval', function( $interval, $sce ) {
    return {
      restrict: 'E',
      scope: {
        title: '=',
        timezone: '='
      },
    
    template: '<div flex ng-if="!tile.error" class="message-content">'
      + '<div> {{ time }}</div><div class="small">{{ date }}</div><div class="small" ></div>'
      + '</div>',

      link: function( scope ) {
        function refresh() {
          var now = moment().tz( scope.timezone );
          scope.time = now.format( 'HH:mm' );
          scope.date = now.format( 'DD MMMM YYYY' );
          
        }

        var timer = $interval( refresh, 1000 );

        scope.$on( '$destroy', function() {
          $interval.cancel( timer );
        } );

        refresh();
      }
    };
  }] );

} )();