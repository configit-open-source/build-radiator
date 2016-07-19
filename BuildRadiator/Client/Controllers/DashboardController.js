( function() {
  'use strict';

  var module = angular.module( 'BuildRadiator' );

  module.controller( 'DashboardController', ['$scope', '$log', '$sce', 'TileConfiguration', 'BuildHub', 'MessageHub', function( $scope, $log, $sce, TileConfiguration, BuildHub, MessageHub ) {
    var self = this;

    self.committerLimit = 11;

    function onMessageUpdate( message ) {
      var tile = self.tiles.find( function( tile ) {
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
      var tile = self.tiles.find( function( tile ) {
        return tile.type === 'project'
          && tile.config.buildName === build.name
          && tile.config.branchName === build.branchName;
      } );

      if ( !tile ) {
        $log.error( 'UNKNOWN PROJECT', build );
        return;
      }

      tile.error = build.error;
    };

    function onProjectUpdate( build ) {
      var tile = self.tiles.find( function( tile ) {
        return tile.type === 'project'
          && tile.config.buildName === build.name
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
      self.tiles.filter( function( t ) {
        return t.type === 'project';
      } ).forEach( function( tile ) {
        BuildHub.server.register( tile.config.buildName, tile.config.branchName );
      } );
    }

    function registerMessages() {
      self.tiles.filter( function( t ) {
        return t.type === 'message';
      } ).forEach( function( tile ) {
        MessageHub.server.get( tile.config.messageKey );
      } );
    }

    TileConfiguration.get().then( function( tiles ) {
      self.tiles = tiles;

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