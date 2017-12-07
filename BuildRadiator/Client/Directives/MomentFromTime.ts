'use strict';

angular
  .module( 'BuildRadiator' )
  .component( 'momentFromTime', {
    bindings: {
      startValue: '<',
      endValue: '<'
    },
    controller: function() {
      var ctrl = this;

      function updateValue() {
        ctrl.formattedValue = ctrl.startValue && ctrl.endValue && window.moment( ctrl.endValue ).from( ctrl.startValue, true );
      }

      ctrl.$onChanges = updateValue;
      ctrl.$onInit = updateValue;
    },
    template: '<span>{{ $ctrl.formattedValue }}</span>'
  } );