(function () {
    "use strict";

    angular.module("app").controller("artistController", artistController);

    function artistController($scope, $http, $mdDialog, contextManagerService) {
        $scope.context = null;
        $scope.artists = [];

        $scope.init = function () {
            var id = getQueryString("id");
            if (id) {
                contextManagerService.set("artist.id", id);
                $scope.get(id);
            } else
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
                $scope.context.givenName = $scope.context.name.given.join(" ");
                $scope.context.familyName = $scope.context.name.family.join(" ");
                if ($scope.context.address.line)
                    $scope.context.addressLines = $scope.context.address.line.join("\r\n");
                $scope.context.district = $scope.context.address.district;
                $scope.context.postalCode = $scope.context.address.postalCode;
                $scope.context.city = $scope.context.address.city;
                $scope.context.state = $scope.context.address.state;
                $scope.context.country = $scope.context.address.country;
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

        $scope.create = function () {
            if ($scope.artistForm.$invalid)
                return;

            var data = {
                "stageName": $scope.context.stageName,
                "name": {
                    "given": $scope.context.givenName.split(" "),
                    "family": $scope.context.familyName.split(" ")
                },
                "address": {
                    "country": $scope.context.country
                },
                "email": $scope.context.email,
            }

            var request = httpRequestBuilder("POST", "/api/artist/", data);
            $http(request).then(function (response) {
                $scope.artists.push(response.data);
                $mdDialog.hide();

                $scope.context = null;
                $scope.artistForm.$setPristine();
                $scope.artistForm.$setUntouched();
            }, function (response) {
                httpErrorHandler(response);
            });
        }

        $scope.update = function update() {
            if ($scope.artistForm.$invalid)
                return;

            var data = {
                "identifier": $scope.context.identifier,
                "stageName": $scope.context.stageName,
                "name": {
                    "given": $scope.context.givenName.split(" "),
                    "family": $scope.context.familyName.split(" ")
                },
                "address": {
                    "line": (($scope.context.addressLines) ? $scope.context.addressLines.split("\r\n") : null),
                    "postalCode": $scope.context.postalCode,
                    "city": $scope.context.city,
                    "state": $scope.context.state,
                    "district": $scope.context.district,
                    "country": $scope.context.country
                },
                "email": $scope.context.email,
                "telecom": $scope.context.telecom
            }

            var request = httpRequestBuilder("PUT", "/api/artist/", data);
            $http(request).then(function (response) {
                $mdDialog.hide();

                $scope.context = response.data;
                $scope.artistForm.$setPristine();
                $scope.artistForm.$setUntouched();
            }, function (response) {
                httpErrorHandler(response);
            });

        }

    }

})();