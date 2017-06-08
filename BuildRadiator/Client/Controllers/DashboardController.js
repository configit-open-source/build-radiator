( function() {
  'use strict';

  var module = angular.module( 'BuildRadiator' );

  module.controller( 'DashboardController', ['$scope', '$log', '$sce', 'TileConfiguration', 'BuildHub', 'MessageHub', function( $scope, $log, $sce, TileConfiguration, BuildHub, MessageHub ) {
    var self = this;

    self.committerLimit = 11;

    function onMessageUpdate( message ) {
      var tile = self.tiles.find( function( tile ) {
        return tile.type === 'Configit.BuildRadiator.Model.MessageTile'
          && tile.messageKey === message.key;
      } );

      if ( !tile ) {
        return;
      }

      message.contentHtml = $sce.trustAsHtml( message.content );

      tile.message = message;
    };

    function onProjectUpdateError( build ) {
      var tile = self.tiles.find( function( tile ) {
        return tile.type === 'Configit.BuildRadiator.Model.BuildTile'
          && tile.build.name === build.name
          && tile.build.branchName === build.branchName;
      } );

      if ( !tile ) {
        $log.error( 'UNKNOWN PROJECT', build );
        return;
      }

      tile.error = build.error;
    };

    function onProjectUpdate( build ) {
      var tile = self.tiles.find( function( tile ) {
        return tile.type === 'Configit.BuildRadiator.Model.BuildTile'
          && tile.build.name === build.name
          && tile.build.branchName === build.branchName;
      } );

      if ( !tile ) {
        $log.error( 'UNKNOWN PROJECT', build );
        return;
      }

      tile.build = build;
      delete tile.error;
    };

    function registerProjects() {
      self.tiles.filter( function( t ) {
        return t.type === 'Configit.BuildRadiator.Model.BuildTile';
      } ).forEach( function( tile ) {
        BuildHub.server.register( tile.build.name, tile.build.branchName );
      } );
    }

    function registerMessages() {
      self.tiles.filter( function( t ) {
        return t.type === 'Configit.BuildRadiator.Model.MessageTile';
      } ).forEach( function( tile ) {
        MessageHub.server.get( tile.messageKey );
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