(function () {
    "use strict";

    angular.module("app").controller("artistController", artistController);

    function artistController($scope, $http, $mdDialog) {
        $scope.context = null;
        $scope.artists = [];

        $scope.init = function () {
            var id = getQueryString("id");
            if (id)
                $scope.get(id);
            else
                $scope.find(null);
        }

        $scope.find = function (q) {
            var url = (q == null) ? "/api/artist/list" : "/api/artist/search/?q=stageName|" + q;
            var request = httpRequestBuilder("GET", url, null);
            $http(request).then(function (response) {
                $scope.artists = response.data;
            }, function (response) {
                httpErrorHandler(response);
            });
        }

        $scope.get = function (id) {
            var request = httpRequestBuilder("GET", "/api/artist/" + id, null);
            $http(request).then(function (response) {
                $scope.artists = [];
                $scope.context = response.data;
            }, function (response) {
                httpErrorHandler(response);
            });
        }

        $scope.delete = function (event, id) {
            var confirm = $mdDialog.confirm()
                  .title("Would you like to delete the artist?")
                  .textContent("The artist and all related data will be deleted. Please notice that this action can not be undone.")
                  .ariaLabel("Confirm")
                  .targetEvent(event)
                  .ok("Yes")
                  .cancel("No");

            $mdDialog.show(confirm).then(function () {
                var request = httpRequestBuilder("DELETE", "/api/artist/" + id, null);
                $http(request).then(function (response) {
                    $scope.find($scope.q);
                }, function (response) {
                    if (response.status == 409)
                    {
                        var alert = $mdDialog.alert()
                            .title("Alert")
                            .textContent("The artist can not be deleted. Please make sure that no accounts, invoices or statements are associated to artist and try again.")
                            .ok("OK");

                        $mdDialog
                            .show(alert)
                            .finally(function () {
                                alert = null;
                            });
                    }
                    else
                        httpErrorHandler(response);
                });
            });
        }

        $scope.create = function (artist) {
            if ($scope.artistForm.$invalid)
                return;

            var data = {
                "stageName": artist.stageName,
                "name": {
                    "given": artist.givenName.split(" "),
                    "family": artist.familyName.split(" ")
                },
                "address": {
                    "country": artist.country
                },
                "email": artist.email
            }

            var request = httpRequestBuilder("POST", "/api/artist/", data);
            $http(request).then(function (response) {
                $scope.artists.push(response.data);
                $mdDialog.hide();

                $scope.artist = null;
                $scope.artistForm.$setPristine();
                $scope.artistForm.$setUntouched();
            }, function (response) {
                httpErrorHandler(response);
            });
        }

    }

})();