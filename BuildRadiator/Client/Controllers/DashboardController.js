( function() {
  'use strict';

  var module = angular.module( 'BuildRadiator' );

  module.controller( 'DashboardController', ['$scope', '$log', '$sce', 'TileConfiguration', 'BuildHub', 'BuildStatisticsHub', 'MessageHub', function( $scope, $log, $sce, TileConfiguration, BuildHub, BuildStatisticsHub, MessageHub ) {
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

    function onBuildStatisticsUpdate( buildDetails ) {
      var tile = self.tiles.find( function( tile ) {
        return tile.type === 'build-statistics'
          && tile.config.buildName === buildDetails.name
          && tile.config.branchName === buildDetails.branchName;
      } );

      if ( !tile ) {
        $log.error( 'UNKNOWN PROJECT', buildDetails );
        return;
      }

      if (! tile.buildDetails) {
        tile.buildDetails = buildDetails;
      }

      tile.buildDetails.statistics = buildDetails.statistics;
    }

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

    function registerBuildStatistics() {
      self.tiles.filter( function( t ) {
        return t.type === 'build-statistics';
      } ).forEach( function( tile ) {
        BuildStatisticsHub.server.register( tile.config.buildName, tile.config.branchName );
      } );      
    }

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

      BuildStatisticsHub.connect( $scope, {
        update: onBuildStatisticsUpdate
      } ).done( registerBuildStatistics );

      MessageHub.connect( $scope, {
        update: onMessageUpdate
      } ).done( registerMessages );
    } );
  }] );

} )();