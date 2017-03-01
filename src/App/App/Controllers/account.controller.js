(function () {
    "use strict";

    angular.module("app").controller("accountController", accountController);

    function accountController($scope, $http, $mdDialog, $q, contextManagerService) {
        $scope.context = null;
        $scope.accounts = [];

        $scope.init = function () {
            var artistId = contextManagerService.get("artist.id");
            if (artistId)
                $scope.find(artistId);
        }

        $scope.find = function (q) {
            var url = (q == null) ? "/api/account/list" : "/api/account/search/?q=artist:reference.uri|/api/artist/" + q;
            var request = httpRequestBuilder("GET", url, null);
            $http(request).then(function (response) {
                $scope.accounts = response.data;
                if ($scope.accounts) {
                    // Resolve the Label resource for each Account.
                    for (var i = 0; i < $scope.accounts.length; i++) {
                        var promise = $scope.resolveLabel(i, $scope.accounts[i].label.uri);
                        promise.then(function (result) {
                            $scope.accounts[result.index].label = result.label;
                        });
                    }
                }
            }, function (response) {
                httpErrorHandler(response);
            });
        }

        $scope.resolveLabel = function (index, uri) {
            var deferred = $q.defer();
            var request = httpRequestBuilder("GET", uri, null);
            $http(request).then(function (response) {
                deferred.resolve({ "index": index, "label": response.data });
            }, function (response) {
                httpErrorHandler(response);
            });

            return deferred.promise;
        }

        $scope.create = function () {
            if ($scope.accountForm.$invalid)
                return;

            var data = {
                "name": $scope.context.name,
                "status": "open",
                "artist": { "uri" : "api/artist/" + contextManagerService.get("artist.id") },
                "label": { "uri": "api/label/" + $scope.context.label }
            }

            var request = httpRequestBuilder("POST", "/api/account/", data);
            $http(request).then(function (response) {
                $scope.accounts.push(response.data);
                $mdDialog.hide();

                $scope.context = null;
                $scope.accountForm.$setPristine();
                $scope.accountForm.$setUntouched();
            }, function (response) {
                httpErrorHandler(response);
            });
        }

    }

})();