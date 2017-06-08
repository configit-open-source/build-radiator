( function() {
  'use strict';

  var module = angular.module( 'BuildRadiator' );

  module.directive( 'clock', ['$interval', '$sce', function( $interval, $sce ) {
    return {
      restrict: 'E',
      scope: {
        timezone: '=',
        title: '='
      },
    
    template: '<div flex ng-if="!tile.error" class="message-content">'
           +  '<div> {{ time }}</div><div class="small">{{ date }}</div><div class="small" ></div>'
           +  '</div>'
           + '<md-grid-tile-footer ><h3><input value="{{ title }}" /><select class="concealed-input" ng-bind-html="timezones" ng-model="timezone"></select></h3></md-grid-tile-footer>',

      link: function( scope ) {
        function refresh() {
          var now = moment().tz( scope.timezone );
         // var timezoneList = '<option selected="1">' + scope.timezone + '</option>';

          scope.time = now.format( 'HH:mm' );
          scope.date = now.format( 'DD MMMM YYYY' );

          scope.timezoneList = '<option selected="1">' + scope.timezone + '</option>';
          moment.tz.names().forEach( zoneOption );

          scope.timezones = $sce.trustAsHtml( scope.timezoneList );
        }

        var timer = $interval( refresh, 1000 );

        scope.$on( '$destroy', function() {
          $interval.cancel( timer );
        } );

        refresh();

        function zoneOption( item, index ) {
          scope.timezoneList += '<option>' + item + '</option>';
        }
      }
    };
  }] );

} )();