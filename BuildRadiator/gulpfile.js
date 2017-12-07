/// <binding ProjectOpened='ts:watch, build:watch' />

var gulp = require( 'gulp' );
var cleanCss = require( 'gulp-clean-css' );
var urlAdjuster = require( 'gulp-css-url-adjuster' );
var concat = require( 'gulp-concat' );
var less = require( 'gulp-less' );
var ngAnnotate = require( 'gulp-ng-annotate' );
var shell = require( 'gulp-shell' );
var ts = require( 'gulp-typescript' );
var uglify = require( 'gulp-uglify' );
var del = require( 'del' );
var path = require( 'path' );
var pump = require( 'pump' );

var tsProject = ts.createProject( 'tsconfig.json' );
var uglifyConfig = require( './uglify-config.json' );

var outputDirectory = 'Client-Release';

/*
  TASKS - Clean
*/
gulp.task( 'clean', function() {
  return del( [outputDirectory + '/**/*'] );
} );

gulp.task( 'clean:css:site', function() {
  return del( [outputDirectory + '/**/site*.css'] );
} );

gulp.task( 'clean:css:vendor', function() {
  return del( [outputDirectory + '/**/vendor*.css'] );
} );

gulp.task( 'clean:js:vendor', function() {
  return del( [outputDirectory + '/**/vendor*.js'] );
} );

/*
  TASKS - Build
*/
gulp.task( 'build:watch', ['build:dev'], function() {
  var lessWatcher = gulp.watch( './Content/**/*.less', ['less:build'] );
  lessWatcher.on( 'change', function( event ) {
    console.log( '[LESS] ' + event.path + ' was ' + event.type + ', building less bundle...');
  } );
} );

gulp.task( 'build:dev', ['css:build', 'less:build', 'js:build'] );

gulp.task( 'build:release', ['css:minify', 'less:minify', 'js:minify', 'ts:minify' ] );

/*
  TASKS - Typescript
*/
gulp.task( 'ts:minify', function() {
  var tsResult = tsProject
    .src()
    .pipe( tsProject() );
  
   return tsResult.js
    .pipe( ngAnnotate( { single_quotes: true } ) )
    .pipe( uglify( uglifyConfig ) )
    .pipe( concat( 'bundle.min.js' ) )
    .pipe( gulp.dest( outputDirectory ) );
} );

gulp.task( 'ts:watch', shell.task( ['tsc --watch'] ) );

/*
  TASKS - CSS
*/
gulp.task( 'css:build', ['clean:css:vendor'], function( cb ) {
  pump( [
    gulp.src( ['./Content/**/*.css'] ),
    urlAdjuster( { prepend: '../Content/' } ),
    concat( 'vendor.css' ),
    gulp.dest( outputDirectory )
  ], cb );
} );

gulp.task( 'css:minify', ['css:build'], function( cb ) {
  pump( [
    gulp.src( ['./' + outputDirectory + '/vendor.css'] ),
    concat( 'vendor.min.css' ),
    cleanCss(),
    gulp.dest( outputDirectory )
  ], cb );
} );

/*
  TASKS - LESS
*/
gulp.task( 'less:build', ['clean:css:site'], function( cb ) {
  pump( [
    gulp.src( ['./Content/site.less'] ),
    less( { paths: ['./Content'], relativeUrls: true } ),
    urlAdjuster( { prepend: '../Content/' } ),
    gulp.dest( outputDirectory )
  ], cb );
} );

gulp.task( 'less:minify', ['less:build'], function( cb ) {
  pump( [
    gulp.src( ['./' + outputDirectory + '/site.css'] ),
    concat( 'site.min.css' ),
    cleanCss(),
    gulp.dest( outputDirectory )
  ], cb );
} );

/*
  TASKS - Javascript
*/
gulp.task( 'js:build', ['clean:js:vendor'], function( cb ) {
  pump( [
    gulp.src( getVendorScriptGlobs( false ) ),
    concat( 'vendor.js' ),
    gulp.dest( outputDirectory )
  ], cb );
} );

gulp.task( 'js:minify', ['js:build'], function( cb ) {
  pump( [ 
    gulp.src( getVendorScriptGlobs( true ) ),
    concat( 'vendor.min.js' ),
    uglify( uglifyConfig ),
    gulp.dest( outputDirectory )
  ], cb );
} );

/*
  Function
*/
function getVendorScriptGlobs( isRelease ) {
  return [
    '!./Scripts/**/*.intellisense.js',
    '!./Scripts/*.min.js',
    
    './Scripts/es6-shim.js',
    './Scripts/jquery-1.12.3.js',
    './Scripts/jquery.signalR-2.2.0.js',
    './Scripts/angular.js',
    './Scripts/moment/moment.js',
    './Scripts/moment/moment-timezone-with-data-2010-2020.js',
    './Scripts/*.js',
    './Scripts/i18n/*.js',
    
    './Scripts/ngyn/ngyn.js',
    './Scripts/ngyn/ngyn-server-connection.js'
  ];
}