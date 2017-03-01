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

        $scope.create = function () {
            if ($scope.labelForm.$invalid)
                return;

            var data = {
                "name": $scope.context.name,
                "email": $scope.context.email,
                "address": {
                    "country": $scope.context.country
                }
            }

            var request = httpRequestBuilder("POST", "/api/label/", data);
            $http(request).then(function (response) {
                $scope.labels.push(response.data);
                $mdDialog.hide();

                $scope.context = null;
                $scope.labelForm.$setPristine();
                $scope.labelForm.$setUntouched();
            }, function (response) {
                httpErrorHandler(response);
            });
        }

    }

})();