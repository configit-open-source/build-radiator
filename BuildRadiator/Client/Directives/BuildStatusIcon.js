( function() {
  'use strict';

  var module = angular.module( 'BuildRadiator' );

  module.directive( 'buildStatusIcon', function() {
    return {
      restrict: 'E',
      scope: {
        status: '='
      },
      template: '<md-icon md-svg-src="{{ iconUrl }}" alt="{{ status }}"></md-icon>',
      link: function( scope ) {
        scope.$watch( 'status', function() {
          scope.iconUrl = scope.status ? 'Content/icons/' + scope.status + '.svg' : null;
        } );
      }
    };
  } );

} )();