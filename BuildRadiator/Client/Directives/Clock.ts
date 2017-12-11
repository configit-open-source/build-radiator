'use strict';

var module = angular.module( 'BuildRadiator' );

function clockDirective( $interval : angular.IIntervalService) {
  return {
    restrict: 'E',
    scope: {
      timezone: '='
    },
    template: '<div>{{ time }}</div><div class="small">{{ date }}</div>',
    link: function( scope: any ) {
      function refresh() {
        var now = new window.moment().tz( scope.timezone );
        scope.time = now.format( 'HH:mm' );
        scope.date = now.format( 'DD MMMM YYYY' );
      }

      var timer = $interval(refresh, 1000);

      scope.$on( '$destroy', () => $interval.cancel( timer ) );

      refresh();
    }
  };
}

module.directive( 'clock', ( clockDirective ) as any );