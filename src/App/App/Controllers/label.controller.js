(function () {
    "use strict";

    angular.module("app").controller("labelController", labelController);

    function labelController($scope, $http, $mdDialog, contextManagerService) {
        $scope.context = null;
        $scope.labels = [];

        $scope.init = function () {
            $scope.find(null);
        }

        $scope.find = function (q) {
            var url = (q == null) ? "/api/label/list" : "/api/label/search/?q=name" + q;
            var request = httpRequestBuilder("GET", url, null);
            $http(request).then(function (response) {
                $scope.labels = response.data;
            }, function (response) {
                httpErrorHandler(response);
            });
        }

    }

})();