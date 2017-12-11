'use strict';

var module = angular.module( 'BuildRadiator' );

module.service( 'BuildHub', ServerConnection => ServerConnection( 'buildHub' ) );