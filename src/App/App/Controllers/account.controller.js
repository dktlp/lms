(function () {
    "use strict";

    angular.module("app").controller("accountController", accountController);

    function accountController($scope, $http, $mdDialog, $q, contextManagerService) {
        $scope.context = null;
        $scope.accounts = [];
        $scope.quarters = [];
        $scope.currentQuarter = null;

        $scope.init = function () {
            var month = Math.floor(new Date().getMonth() / 3) + 1;
            var quarter = Math.ceil(month / 3);
            var year = new Date().getFullYear();

            var q = quarter, y = year;
            for (var i = 0; i < 2; i++) {
                q++;
                if (q > 4) {
                    q = 1;
                    y++;
                }

                $scope.quarters.push("Q" + q + "-" + y);
            }

            $scope.quarters.reverse();
            $scope.quarters.push("Q" + quarter + "-" + year);

            q = quarter, y = year;
            for (var i = 0; i < 4; i++) {
                q--;
                if (q <= 0) {
                    q = 4;
                    y--;
                }

                $scope.quarters.push("Q" + q + "-" + y);
            }

            $scope.currentQuarter = "Q" + quarter + "-" + year;

            var accountId = contextManagerService.get("account.id");
            if (accountId)
                $scope.get(accountId);
            else {
                var artistId = contextManagerService.get("artist.id");
                if (artistId)
                    $scope.find(artistId);
                else
                    $scope.find(null);
            }
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
                    // Resolve the Account resource for each Account.
                    for (var i = 0; i < $scope.accounts.length; i++) {
                        var promise = $scope.resolveArtist(i, $scope.accounts[i].artist.uri);
                        promise.then(function (result) {
                            $scope.accounts[result.index].artist = result.artist;
                        });
                    }
                }
            }, function (response) {
                httpErrorHandler(response);
            });
        }

        $scope.get = function (id) {
            var request = httpRequestBuilder("GET", "/api/account/" + id, null);
            $http(request).then(function (response) {
                $scope.accounts = [];
                $scope.context = response.data;
                $scope.context.quarter = $scope.currentQuarter;
                // Resolve the Label resource for Account.
                var promise = $scope.resolveLabel(0, $scope.context.label.uri);
                promise.then(function (result) {
                    $scope.context.label = result.label;
                });
                // Resolve the Account resource for Account.
                var promise = $scope.resolveArtist(0, $scope.context.artist.uri);
                promise.then(function (result) {
                    $scope.context.artist = result.artist;
                });
            }, function (response) {
                httpErrorHandler(response);
            });
        }

        $scope.resolveLabel = function (index, uri) {
            var deferred = $q.defer();
            var request = httpRequestBuilder("GET", uri, null, true);
            $http(request).then(function (response) {
                deferred.resolve({ "index": index, "label": response.data });
            }, function (response) {
                httpErrorHandler(response);
            });

            return deferred.promise;
        }

        $scope.resolveArtist = function (index, uri) {
            var deferred = $q.defer();
            var request = httpRequestBuilder("GET", uri, null, true);
            $http(request).then(function (response) {
                deferred.resolve({ "index": index, "artist": response.data });
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

        $scope.delete = function (event, id) {
            var confirm = $mdDialog.confirm()
                  .title("Would you like to delete the account?")
                  .textContent("The account and all related data will be deleted. Please notice that this action can not be undone.")
                  .ariaLabel("Confirm")
                  .targetEvent(event)
                  .ok("Yes")
                  .cancel("No");

            $mdDialog.show(confirm).then(function () {
                var request = httpRequestBuilder("DELETE", "/api/account/" + id, null);
                $http(request).then(function (response) {
                    var artistId = contextManagerService.get("artist.id");
                    if (artistId)
                        $scope.find(artistId);
                }, function (response) {
                    if (response.status == 409) {
                        var alert = $mdDialog.alert()
                            .title("Alert")
                            .textContent("The account can not be deleted as there are transactions associated with it.")
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

        $scope.createTransaction = function (type) {
            if ($scope.transactionForm.$invalid)
                return;

            var data = {
                "account": { "uri": "api/account/" + $scope.context.identifier },
                "status": "committed",
                "type": type,
                "amount": $scope.context.amount,
                "quarter": $scope.context.quarter,                
                "description": $scope.context.description
            }

            var request = httpRequestBuilder("POST", "/api/account/transaction/", data);
            $http(request).then(function (response) {
                $scope.accounts.push(response.data);
                $mdDialog.hide();

                $scope.context = null;
                $scope.transactionForm.$setPristine();
                $scope.transactionForm.$setUntouched();
            }, function (response) {
                httpErrorHandler(response);
            });
        }

        $scope.setContext = function (account) {
            if (account)
                contextManagerService.set("account.id", account.identifier);
            else {
                contextManagerService.set("account.id", null);
                contextManagerService.set("artist.id", null);
            }
        }

    }

})();