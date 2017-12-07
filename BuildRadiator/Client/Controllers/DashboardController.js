( function() {
  'use strict';

  var module = angular.module( 'BuildRadiator' );

  module.controller( 'DashboardController', ['$scope', '$window', '$log', '$sce', 'TileConfiguration', 'BuildHub', 'MessageHub', function( $scope, $window, $log, $sce, TileConfiguration, BuildHub, MessageHub ) {
    var ctrl = this;

    ctrl.committerLimit = 11;

    function onMessageUpdate( message ) {
      var tile = ctrl.tiles.find( function( tile ) {
        return tile.type === 'message'
          && tile.config.messageKey === message.key;
      } );

      if ( !tile ) {
        return;
      }

      message.contentHtml = $sce.trustAsHtml( message.content );

      tile.message = message;
    };

    function onProjectUpdateError( build ) {
      var tile = ctrl.tiles.find( function( tile ) {
        return tile.type === 'project'
          && tile.config.buildId === build.buildId
          && tile.config.branchName === build.branchName;
      } );

      if ( !tile ) {
        $log.error( 'UNKNOWN PROJECT', build );
        return;
      }

      tile.error = build.error;
    };

    function onProjectUpdate( build ) {
      var tile = ctrl.tiles.find( function( tile ) {
        return tile.type === 'project'
          && tile.config.buildId === build.id
          && tile.config.branchName === build.branchName;
      } );

      if ( !tile ) {
        $log.error( 'UNKNOWN PROJECT', build );
        return;
      }

      tile.project = build;
      delete tile.error;
    };

    function registerProjects() {
      ctrl.tiles.filter( function( t ) {
        return t.type === 'project';
      } ).forEach( function( tile ) {
        BuildHub.server.register( tile.config.buildId, tile.config.branchName );
      } );
    }

    function registerMessages() {
      ctrl.tiles.filter( function( t ) {
        return t.type === 'message';
      } ).forEach( function( tile ) {
        MessageHub.server.get( tile.config.messageKey );
      } );
    }

    ctrl.gotoProject = function( url ) {
      $window.location = url;
    }

    TileConfiguration.get().then( function( tiles ) {
      ctrl.tiles = tiles;

      BuildHub.connect( $scope, {
        update: onProjectUpdate,
        updateError: onProjectUpdateError
      } ).done( registerProjects );

      MessageHub.connect( $scope, {
        update: onMessageUpdate
      } ).done( registerMessages );
    } );
  }] );

} )();