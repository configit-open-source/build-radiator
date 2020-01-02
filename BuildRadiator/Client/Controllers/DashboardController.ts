'use strict';

var module = angular.module( 'BuildRadiator' );

var dashboardController = function( $scope, $window : angular.IWindowService, $log : angular.ILogService, $sce : angular.ISCEService, $timeout : angular.ITimeoutService, TileLayoutHub, BuildHub, MessageHub, TileLayoutConfig ) {
  const ctrl = this;
  const timeoutInMinutes = 30;

  ctrl.committerLimit = 11;
  ctrl.lastUpdated = new Date();
  
  let timerToken;

  function refresh() {
    $log.log( 'Reloading...' );
    $window.location.reload();
  }

  function resetTimeout() {
    ctrl.lastUpdated = new Date();
    if( timerToken ) {
      $timeout.cancel( timerToken );
      timerToken = $timeout( refresh, timeoutInMinutes * 60 * 1000 );
    }
  }

  function onMessageUpdate( message ) {
    const tiles = ctrl.tiles.filter( t => t.type === 'message' && t.config.messageKey === message.key );

    if ( tiles.length ) {
      message.contentHtml = $sce.trustAsHtml( message.content );
    }

    tiles.forEach( t => t.message = message );
  };

  function onProjectUpdateError( build ) {
    resetTimeout();

    const projectTiles = ctrl.tiles.filter( t =>
      t.type === 'project'
      && t.config.buildId === build.buildId
      && t.config.branchName === build.branchName );

    projectTiles.forEach( t => t.error = build.error );
      
    const primaryProjectTiles = ctrl.tiles.filter( t => 
      t.type === 'dual-project'
      && t.config.primaryBuildId === build.buildId
      && t.config.primaryBranchName === build.branchName );

    primaryProjectTiles.forEach( t => t.primaryError = build.error );
      
    const secondaryProjectTiles = ctrl.tiles.filter( t =>
      t.type === 'dual-project'
      && t.config.secondaryBuildId === build.buildId
      && t.config.secondaryBranchName === build.branchName );

    secondaryProjectTiles.forEach( t => t.secondaryError = build.error );

    if ( !projectTiles.length && !primaryProjectTiles.length && !secondaryProjectTiles.length ) {
      $log.error( 'onProjectError: UNKNOWN PROJECT', build );

    }
  };

  function onProjectUpdate( build ) {
    resetTimeout();

    var projectTiles = ctrl.tiles.filter( t => 
      t.type === 'project'
      && t.config.buildId === build.id
      && t.config.branchName === build.branchName );

    projectTiles.forEach( t => {
      t.project = build;
      delete t.error;
    } );
      
    var primaryProjectTiles = ctrl.tiles.filter( t => 
      t.type === 'dual-project'
      && t.config.primaryBuildId === build.id
      && t.config.primaryBranchName === build.branchName );

    primaryProjectTiles.forEach( t => {
      t.primaryProject = build;
      delete t.primaryError;
    } );
      
    var secondaryProjectTiles = ctrl.tiles.filter( t => 
      t.type === 'dual-project'
      && t.config.secondaryBuildId === build.id
      && t.config.secondaryBranchName === build.branchName );

    secondaryProjectTiles.forEach( t => {
      t.secondaryProject = build;
      delete t.secondaryError;
    } );

    if ( !projectTiles.length && !primaryProjectTiles.length && !secondaryProjectTiles.length ) {
      $log.error( 'onProjectUpdate: UNKNOWN PROJECT', build );
    }
  };

  function registerProjects() {
    if( ctrl.previousTiles ) {
      ctrl.previousTiles.filter( t => t.type === 'project' ).forEach( t => {
        BuildHub.server.unregister( t.config.buildId, t.config.branchName );
      } );

      ctrl.previousTiles.filter( t => t.type === 'dual-project' ).forEach( t => {
        BuildHub.server.unregister( t.config.primaryBuildId, t.config.primaryBranchName );
        BuildHub.server.unregister( t.config.secondaryBuildId, t.config.secondaryBranchName );
      } );
    }

    ctrl.tiles.filter( t => t.type === 'project' ).forEach( t => {
      BuildHub.server.register( t.config.buildId, t.config.branchName );
    } );

    ctrl.tiles.filter( t => t.type === 'dual-project' ).forEach( t => {
      BuildHub.server.register( t.config.primaryBuildId, t.config.primaryBranchName );
      BuildHub.server.register( t.config.secondaryBuildId, t.config.secondaryBranchName );
    } );
  }

  function registerMessages() {
    ctrl.tiles.filter( t => t.type === 'message' ).forEach( t => {
      MessageHub.server.get( t.config.messageKey );
    } );
  }

  function onTileLayoutUpdate( layoutName ) {
    if( TileLayoutConfig.layoutName.toLowerCase() !== layoutName.toLowerCase() ) {
      return;
    }

    TileLayoutHub.server.get( TileLayoutConfig.layoutName ).then( tiles => {
      ctrl.previousTiles = ctrl.tiles;
      ctrl.tiles = tiles;

      registerProjects();
      registerMessages();
    } );
  }

  ctrl.gotoProject = ( url : string, e ) => {
    if ( !url ) {
      return;
    }

    $window.location.href = url;
    e.stopPropagation();
  }

  TileLayoutHub.connect( $scope, {
    update: onTileLayoutUpdate
  } ).done( () => {
    TileLayoutHub.server.get( TileLayoutConfig.layoutName ).then( tiles => {
      ctrl.previousTiles = ctrl.tiles;
      ctrl.tiles = tiles;

      BuildHub.connect( $scope, {
        update: onProjectUpdate,
        updateError: onProjectUpdateError
      } ).done( registerProjects );

      MessageHub.connect( $scope, {
        update: onMessageUpdate
      } ).done( registerMessages );

      resetTimeout();
    } );
  } );
} 

module.controller( 'DashboardController', ( dashboardController ) as any );