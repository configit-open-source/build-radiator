'use strict';

var module = angular.module( 'BuildRadiator' );
  
module.factory( 'TileConfiguration', ( $q, $http ) => ( {
  get() {
    var deferred = $q.defer();

    $http.get( 'api/tile' ).then( response => {
      deferred.resolve( response.data );
    }, error => {
      deferred.reject( error );
    } );

    return deferred.promise;
  }
} ) );