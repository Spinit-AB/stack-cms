var angular = require('angular');

(function () {

    angular.module('Spinit.Stack.CMS')
        .controller("home.controller", ['$scope', function ($scope) {
            $scope.test = "Test text";
        }]);

})();