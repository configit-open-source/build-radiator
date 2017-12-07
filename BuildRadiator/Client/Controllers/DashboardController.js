( function() {
  'use strict';

  var module = angular.module( 'BuildRadiator' );

  module.controller( 'DashboardController', ['$scope', '$window', '$log', '$sce', 'TileConfiguration', 'BuildHub', 'MessageHub', function( $scope, $window, $log, $sce, TileConfiguration, BuildHub, MessageHub ) {
    var ctrl = this;

    ctrl.committerLimit = 11;

    function onMessageUpdate( message ) {
      var tiles = ctrl.tiles.filter( function( tile ) {
        return tile.type === 'message'
          && tile.config.messageKey === message.key;
      } );

      if ( tiles.length ) {
        message.contentHtml = $sce.trustAsHtml( message.content );
      }

      tiles.forEach( function( t ) {
        t.message = message;  
      } );
    };

    function onProjectUpdateError( build ) {
      var projectTiles = ctrl.tiles.filter( function( tile ) {
        return tile.type === 'project'
          && tile.config.buildId === build.buildId
          && tile.config.branchName === build.branchName;
      } );

      projectTiles.forEach( function( t ) {
        t.error = build.error;
      } );
      
      var primaryProjectTiles = ctrl.tiles.filter( function( tile ) {
        return tile.type === 'dual-project'
          && tile.config.primaryBuildId === build.buildId
          && tile.config.primaryBranchName === build.branchName;
      } );

      primaryProjectTiles.forEach( function( t ) {
        t.primaryError = build.error;
      } );
      
      var secondaryProjectTiles = ctrl.tiles.filter( function( tile ) {
        return tile.type === 'dual-project'
          && tile.config.secondaryBuildId === build.BuildId
          && tile.config.secondaryBranchName === build.branchName;
      } );

      secondaryProjectTiles.forEach( function( t ) {
        t.secondaryError = build.error;
      } );

      if ( !projectTiles.length && !primaryProjectTiles.length && !secondaryProjectTiles.length ) {
        $log.error( 'onProjectError: UNKNOWN PROJECT', build );
      }
    };

    function onProjectUpdate( build ) {
      var projectTiles = ctrl.tiles.filter( function( tile ) {
        return tile.type === 'project'
          && tile.config.buildId === build.id
          && tile.config.branchName === build.branchName;
      } );

      projectTiles.forEach( function( t ) {
        t.project = build;
        delete t.error;
      } );
      
      var primaryProjectTiles = ctrl.tiles.filter( function( tile ) {
        return tile.type === 'dual-project'
          && tile.config.primaryBuildId === build.id
          && tile.config.primaryBranchName === build.branchName;
      } );

      primaryProjectTiles.forEach( function( t ) {
        t.primaryProject = build;
        delete t.primaryError;
      } );
      
      var secondaryProjectTiles = ctrl.tiles.filter( function( tile ) {
        return tile.type === 'dual-project'
          && tile.config.secondaryBuildId === build.id
          && tile.config.secondaryBranchName === build.branchName;
      } );

      secondaryProjectTiles.forEach( function( t ) {
        t.secondaryProject = build;
        delete t.secondaryError;
      } );

      if ( !projectTiles.length && !primaryProjectTiles.length && !secondaryProjectTiles.length ) {
        $log.error( 'onProjectUpdate: UNKNOWN PROJECT', build );
      }
    };

    function registerProjects() {
      ctrl.tiles.filter( function( t ) {
        return t.type === 'project';
      } ).forEach( function( tile ) {
        BuildHub.server.register( tile.config.buildId, tile.config.branchName );
      } );

      ctrl.tiles.filter( function( t ) {
        return t.type === 'dual-project';
      } ).forEach( function( tile ) {
        BuildHub.server.register( tile.config.primaryBuildId, tile.config.primaryBranchName );
        BuildHub.server.register( tile.config.secondaryBuildId, tile.config.secondaryBranchName );
      } );
    }

    function registerMessages() {
      ctrl.tiles.filter( function( t ) {
        return t.type === 'message';
      } ).forEach( function( tile ) {
        MessageHub.server.get( tile.config.messageKey );
      } );
    }

    ctrl.gotoProject = function( url, e ) {
      if ( !url ) {
        return;
      }

      $window.location = url;
      e.stopPropagation();
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