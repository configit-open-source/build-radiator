( function() {
  'use strict';

  var module = angular.module( 'BuildRadiator' );

  module.directive( 'buildStatistics', ['$interval', function() {
    return {
      restrict: 'E',
      scope: {
        buildDetails: '='
      },
      template: '<div><div>{{ getLast().statementCodeCoverage }}%</div><div class="small {{ getChangeClass() }}">{{ getChangeText() }}</div></div>',
      link: function( scope ) {
        scope.getLast = function() {
          if ( !scope.buildDetails ) {
            return null;
          }

          return scope.buildDetails.statistics[scope.buildDetails.statistics.length - 1];
        }

        scope.getChangeText = function() {
          if ( !scope.buildDetails || scope.buildDetails.statistics.length < 2 ) {
            return '';
          }

          var change = getLastChange();
          return ( change >= 0 ? '+' : '' ) + change + '%';
        }

        scope.getChangeClass = function() {
          if ( !scope.buildDetails || scope.buildDetails.statistics.length < 2 ) {
            return '';
          }

          var change = getLastChange();
          return change >= 0 ? 'positive-change' : 'negative-change';
        }

        function getLastChange() {
          var statistics = scope.buildDetails.statistics;
          if ( statistics.length < 2 ) {
            return 0;
          }

          return parseFloat( ( statistics[statistics.length - 1].statementCodeCoverage -
            statistics[statistics.length - 2].statementCodeCoverage ).toFixed( 1 ) );
        }
      }
    };
  }] );

} )();