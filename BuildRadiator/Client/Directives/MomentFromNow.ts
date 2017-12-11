'use strict';

function momentFromNowController( $interval: angular.IIntervalService, $scope: any ) {
  var ctrl = this;
  var intervalToken;
  var interval = 1000;

  function updateValue() {
    ctrl.formattedValue = ctrl.value && window.moment( ctrl.value ).fromNow();
  }

  ctrl.$onInit = () => {
    // $scope.apply manually after interval to reduce impact as $interval uses $rootScope
    intervalToken = $interval( () => {
      updateValue();
      $scope.$digest();
    },
    interval,
    0, // number of times to repeat. 0 = indefinite
    false );

    updateValue();
  }

  ctrl.$onChanges = updateValue;

  ctrl.$onDestroy = () => $interval.cancel( intervalToken );
}

angular
  .module( 'BuildRadiator' )
  .component( 'momentFromNow', {
    bindings: {
      value: '<'
    },
    controller: ( momentFromNowController as any ),
    template: '<span>{{ $ctrl.formattedValue }}</span>'
  } );