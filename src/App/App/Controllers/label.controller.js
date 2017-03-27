(function () {
    "use strict";

    angular.module("app").controller("labelController", labelController);

    function labelController($scope, $http, $mdDialog, contextManagerService) {
        $scope.context = null;
        $scope.labels = [];

        $scope.init = function () {
            var id = contextManagerService.get("label.id");
            if (id) {
                $scope.get(id);
            } else
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

        $scope.get = function (id) {
            var request = httpRequestBuilder("GET", "/api/label/" + id, null);
            $http(request).then(function (response) {
                $scope.labels = [];
                $scope.context = response.data;
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

        $scope.delete = function (event, id) {
            var confirm = $mdDialog.confirm()
                  .title("Would you like to delete the label?")
                  .textContent("The label and all related data will be deleted. Please notice that this action can not be undone.")
                  .ariaLabel("Confirm")
                  .targetEvent(event)
                  .ok("Yes")
                  .cancel("No");

            $mdDialog.show(confirm).then(function () {
                var request = httpRequestBuilder("DELETE", "/api/label/" + id, null);
                $http(request).then(function (response) {
                    $scope.find(null);
                }, function (response) {
                    if (response.status == 409) {
                        var alert = $mdDialog.alert()
                            .title("Alert")
                            .textContent("The label can not be deleted as there are artist accounts associated with it.")
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

        $scope.update = function update() {
            if ($scope.labelForm.$invalid)
                return;

            var data = {
                "identifier": $scope.context.identifier,
                "name": $scope.context.name,
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

            var request = httpRequestBuilder("PUT", "/api/label/", data);
            $http(request).then(function (response) {
                $mdDialog.hide();

                $scope.context = response.data;
                $scope.labelForm.$setPristine();
                $scope.labelForm.$setUntouched();
            }, function (response) {
                httpErrorHandler(response);
            });

        }

        $scope.setContext = function(label)
        {
            if (label)
                contextManagerService.set("label.id", label.identifier);
            else
                contextManagerService.set("label.id", null);
        }

    }

})();