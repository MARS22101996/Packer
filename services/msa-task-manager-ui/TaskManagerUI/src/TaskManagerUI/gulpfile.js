/// <binding BeforeBuild='rungulp' />
var gulp = require("gulp"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify"),
    del = require("del"),
    es = require("event-stream"),
    gulpConfig = require("./gulpconfig.json");

var gulpManager = {
    cleanMinFiles: function (taskName, gulpConfig) {
        gulp.task(taskName, function (cb) {
            gulpConfig.cleaningTasksData.forEach(function(item) {
                var folderPath = gulpConfig.webroot + item.filesFolderName;
                var filesPath = folderPath + item.files;
                del.sync([filesPath, "!" + folderPath]);
            });
        });
    },

    minimizeFiles: function(taskName, gulpConfig) {
        gulp.task(taskName, function () {
            return es.merge(gulpConfig.minimizationTasksData.map(function(item) {
                var splitedOutputFilePath = item.outputFile.split(".");
                var outputFileExtension = splitedOutputFilePath[splitedOutputFilePath.length - 1].toLocaleLowerCase();
                var outputFilePath = gulpConfig.webroot + item.outputFile;

                var inputFilesPaths = item.inputFiles.slice();
                for (var i = 0; i < inputFilesPaths.length; i++) {
                    inputFilesPaths[i] = gulpConfig.webroot + inputFilesPaths[i];
                }

                var minMethod = outputFileExtension == "js" ? uglify() : cssmin();
                return gulp
                    .src(inputFilesPaths, { base: "." })
                    .pipe(minMethod)
                    .pipe(concat(outputFilePath))
                    .pipe(gulp.dest("."));
            }));
        });
    }
};

var cleanMinFilesTaskName = "cleanMinFiles";

var minimizeFilesTaskName = "minimizeFiles";

gulpManager.cleanMinFiles(cleanMinFilesTaskName, gulpConfig);

gulpManager.minimizeFiles(minimizeFilesTaskName, gulpConfig);

gulp.task("rungulp", [cleanMinFilesTaskName, minimizeFilesTaskName]);