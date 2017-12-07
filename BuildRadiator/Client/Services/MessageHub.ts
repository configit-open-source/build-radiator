'use strict';

var module = angular.module( 'BuildRadiator' );

module.service( 'MessageHub', ServerConnection => ServerConnection( 'messageHub' ) );