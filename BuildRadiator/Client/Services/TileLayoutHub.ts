'use strict';

var module = angular.module( 'BuildRadiator' );
  
module.service( 'TileLayoutHub', ServerConnection => ServerConnection( 'tileLayoutHub' ) );