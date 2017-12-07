'use strict';

var module = angular.module( 'BuildRadiator' );

module.directive( 'buildStatusIcon', () => ( {
  restrict: 'E',
  scope: {
    status: '='
  },
  template: '<md-icon md-svg-src="{{ iconUrl }}" alt="{{ status }}"></md-icon>',
  link: function( scope : any ) {
    scope.$watch( 'status', () => {
      scope.iconUrl = scope.status ? 'Content/icons/' + scope.status + '.svg' : null;
    } );
  }
} ) );