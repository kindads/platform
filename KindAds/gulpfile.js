/*
This file is the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. https://go.microsoft.com/fwlink/?LinkId=518007
*/

const gulp = require('gulp'),
      sass = require('gulp-sass'),
      sourcemaps = require('gulp-sourcemaps'),
      pug = require('gulp-pug');


const plumber = require('gulp-plumber');

gulp.task('styles', function () {
  return gulp.src('scss/**/*.scss')
    .pipe(sourcemaps.init())
    .pipe(sass({ outputStyle: 'compressed' }).on('error', sass.logError))
    //.pipe(sass().on('error', sass.logError))
    .pipe(sourcemaps.write('.map'))
    .pipe(gulp.dest('Content'));
});

gulp.task('pug', function () {
  return gulp.src([
    'Static Files/views/**/*.pug',
    '!Static Files/views/partials/**/*.pug',
    '!Static Files/views/templates/**/*.pug'
  ])
    .pipe(plumber())
    .pipe(pug({
      doctype: 'html',
      pretty: true
    }))
    .pipe(gulp.dest('Static Files/'));
});

gulp.task('watch', function () {
  gulp.watch('Static Files/views/**/*.pug', ['pug']);
  gulp.watch('scss/**/*.scss', ['styles']);
});

gulp.task('default', function () {
    // place code for your default task here
});
