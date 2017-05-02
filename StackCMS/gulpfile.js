/// <binding ProjectOpened='watch' />
var gulp = require('gulp');
var del = require('del');
var series = require('stream-series');
var config = require('./gulp.config')();
var concat = require('gulp-concat');
var mainNpmFiles = require('gulp-main-npm-files');
var cleanCSS = require('gulp-clean-css');
var autoprefixer = require('gulp-autoprefixer');
var watch = require('gulp-watch');
var sort = require('gulp-sort');
var $ = require('gulp-load-plugins')({ lazy: true });
var flatten = require('gulp-flatten');

/**
 * Un-comment following line to be able to use semantic-ui's own gulp tasks.
 */
// var semantic = require('./semantic.config.js')(gulp);

gulp.task('default', ['watch']);

/**
 * WATCH DEV!
 * Watches changes in js, scss/less and injects sources into index.html
 */
gulp.task('watch', ['dev'], () => {
    watch(config.jsSrc.concat(
        [
            config.scssSrcWatch,
            config.lessSrcWatch,
            config.semanticSiteWatch,
            config.semanticThemeWatch
        ]), () => {
            gulp.start('dev');
        });
});

/**
 * DEV!
 * Injects js and css into index.html
 */
gulp.task('dev', ['inject-js', 'inject-css'], () => {
    log(config.index + ' updated');
});

gulp.task('inject-js', () => {
    var jsFilter = $.filter('**/*.js');

    var target = gulp.src(config.index);
    var appSources = gulp.src(config.jsSrc, { read: false })
        .pipe(sort(jsSort));
    var vendorSources = gulp.src(mainNpmFiles())
        .pipe(jsFilter);
    return target.pipe($.inject(series(vendorSources, appSources)))
        .pipe(gulp.dest(config.indexRoot));
});

gulp.task('inject-css', ['compile-styles'], () => {
    var cssFilter = $.filter('**/*.css');
    var target = gulp.src(config.index);
    var appSources = gulp.src(config.cssBuild, { read: false })
        .pipe(sort());
    var vendorSources = gulp.src(mainNpmFiles())
        .pipe(cssFilter);
    return target.pipe($.inject(series(vendorSources, appSources)))
        .pipe(gulp.dest(config.indexRoot));
});

/**
 * BUILD!
 * Compile js and css and inject into index for production
 */
gulp.task('build', ['inject-minified-js', 'inject-minified-css', 'move-fonts', 'move-minify-imgs'], () => {
    log('Build complete');
});

gulp.task('inject-minified-js', ['minify-js'], () => {
    var target = gulp.src(config.index);
    var build = gulp.src(config.jsDist, { read: false });
    return target.pipe($.inject(build))
        .pipe(gulp.dest(config.indexRoot));
});

gulp.task('inject-minified-css', ['minify-css'], () => {
    var target = gulp.src(config.index);
    var src = gulp.src(config.cssDist, { read: false });
    return target
        .pipe($.inject(src))
        .pipe(gulp.dest(config.indexRoot));
});

/**
 * Copy fonts in npm packages
 * Set/Override path to fonts in bower.json 
 * "overrides": {
    "fontawesome": {
      "main": [
        "./fonts/*.*"
      ]
    }
   }
 */
gulp.task('move-fonts', ['clean-dist'], () => {
    var fontsFilter = () => $.filter(['**/fonts/**']);

    var npmFonts = gulp.src(mainNpmFiles())
        .pipe(fontsFilter())
        .pipe(flatten({ newPath: 'fonts' }));
    var stylesFonts = gulp.src(config.styles)
        .pipe(fontsFilter());

    return series(npmFonts, stylesFonts)
        .pipe(gulp.dest(config.stylesDistDir));
});

gulp.task('move-minify-imgs', ['clean-dist'], () => {
    log('Copying and compressing images');

    var imagesFilter = $.filter(['**/img/**']);

    return gulp.src(config.styles)
        .pipe(imagesFilter)
        .pipe($.imagemin({ optimizationLevel: 4 }))
        .pipe(gulp.dest(config.stylesDistDir));
});

gulp.task('minify-js', ['clean-dist'], () => {
    var appSource = gulp.src(config.jsSrc)
        .pipe(sort(jsSort));
    var jsFilter = $.filter('**/*.js');

    var vendorSource = gulp.src(mainNpmFiles())
        .pipe(jsFilter);

    return series(vendorSource, appSource)
        .pipe(concat('spinit.stack.min.js'))
        .pipe($.buffer())
        .pipe($.rev())
        .pipe($.uglify())
        .pipe(gulp.dest(config.jsDistDir));
});

gulp.task('minify-css', ['compile-styles', 'clean-dist'], () => {
    var appCss = gulp
        .src(config.cssBuild)
        .pipe(sort());

    var cssFilter = $.filter('**/*.css');
    var vendorCss = gulp.src(mainNpmFiles())
        .pipe(cssFilter);

    return series(vendorCss, appCss)
        .pipe(concat('spinit.stack.min.css'))
        .pipe(cleanCSS())
        .pipe($.buffer())
        .pipe($.rev())
        .pipe(gulp.dest(config.cssDistDir));
});

gulp.task('compile-styles', ['clean-css-build'], () => {
    var scss = gulp.src(config.styles)
        .pipe($.filter(['**/scss/app.scss']))
        .pipe($.sass());
    var less = gulp.src(config.styles)
        .pipe($.filter(['**/less/app.less']))
        .pipe($.less());
    return series(scss, less)
        .pipe(concat('app.css'))
        .pipe(autoprefixer({
  				browsers: ['last 2 versions'],
  				cascade: false
			  }))
        .pipe(gulp.dest(config.cssBuildDir));
});

gulp.task('clean-dist', (done) => {
    clean(config.distDir, done);
});

gulp.task('clean-css-build', (done) => {
    clean(config.cssBuildDir, done);
});

/**
 * Performs static code analysis on all javascript files. Runs jshint and jscs.
 */
gulp.task('vet', () => {
    log('Analyzing source with JSHint and JSCS');

    return gulp
        .src(config.jsSrc)
        .pipe($.jscs())
        .pipe($.jshint())
        .pipe($.jshint.reporter('jshint-stylish', { verbose: true }))
        .pipe($.jshint.reporter('fail'));
});

///////////////////////
function clean(path, done) {
    log('Cleaning: ' + $.util.colors.blue(path));
    del(path)
        .then(() => {
            done();
        });
}

function log(msg) {
    if (typeof (msg) === 'object') {
        for (var item in msg) {
            if (msg.hasOwnProperty(item)) {
                $.util.log($.util.colors.blue(msg[item]));
            }
        }
    } else {
        $.util.log($.util.colors.blue(msg));
    }
}

function jsSort(file1, file2) {
    if (file1.path.indexOf('module.js') > 0 && file2.path.indexOf('module.js') > 0) {
        return file1.path.localeCompare(file2.path);
    } else if (file1.path.indexOf('module.js') > 0) {
        return -1;
    } else if (file2.path.indexOf('module.js') > 0) {
        return 1;
    }
    return file1.path.localeCompare(file2.path);
}