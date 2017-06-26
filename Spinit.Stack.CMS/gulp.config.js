module.exports = function () {
  var root = './';
  var distDir = root + 'dist/';
  var jsDistDir = distDir + 'js/';
  var jsSrcDir = root + 'Features/';
  var stylesDir = root + 'assets/styles/';
  var stylesFontDir = stylesDir + 'fonts/';
  var fontAwesomePath = 'node_modules/font-awesome/';
  
  var cssBuildDir = stylesDir + 'css/';

  var stylesDistDir = distDir + 'styles/';
  var cssDistDir = stylesDistDir + 'css/';
  var index = root + 'Views/Shared/_Layout.cshtml';
  var indexRoot = root + 'Views/Shared/';
  var bower = root + 'bower.json';
  var npm = root + 'package.json';

    var manualNodeDependenciesJs =
    [
        'node_modules/angular/angular.js',
        'node_modules/angular-animate/angular-animate.js',
        'node_modules/angular-ui-bootstrap/dist/ui-bootstrap-tpls.js'
    ];
  var config = {
    distDir: distDir,
    jsDistDir: jsDistDir,
    jsDist: jsDistDir + '*.js',
    jsSrc: [jsSrcDir + '**/*.js'],

    htmlSrc: jsSrcDir + '**/*.html',

    scssSrcWatch: stylesDir + 'scss/**/*.scss',
    lessSrcWatch: stylesDir + 'less/**/*.less',
    cssBuildDir: cssBuildDir,
    cssBuild: cssBuildDir + '*.css',
    cssDistDir: cssDistDir,
    cssDist: cssDistDir + '*.css',
    stylesDistDir: stylesDistDir,
    stylesDir: stylesDir,
    styles: stylesDir + '**/*.*',
	fontAwesomePath : fontAwesomePath + '**/*.*',
    bower: bower,
	npm: npm,
    index: index,
	indexRoot: indexRoot,
    root: root,
    manualNodeDependenciesJs: manualNodeDependenciesJs,
    semanticSiteWatch: root + 'assets/semantic/src/site/**/*.{overrides,variables}',
    semanticThemeWatch: root + 'assets/semantic/src/themes/**/*.{overrides,variables}'
  }

  return config;
};