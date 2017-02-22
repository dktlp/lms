(function () {
    "use strict";

    angular.module("app").controller("artistController", artistController);

    function artistController($scope, $mdDialog) {

        $scope.artists = [
            {
                "id": 1,
                "stageName": "Tomtek",
                "name": { "text": "Thomas Lykke Petersen" },
                "address": {"country": "Denmark"},
                "email": "thomas@ktrecordings.com"
            },
            {
                "id": 2,
                "stageName": "Axiom",
                "name": { "text": "Lukas Kozelka" },
                "address": { "country": "Switzerland" },
                "email": "axiom23@hotmail.com"
            },
            {
                "id": 3,
                "stageName": "NickBee",
                "name": { "text": "Mykola Bogomolov" },
                "address": { "country": "Ukraine" },
                "email": "nickbee2007@gmail.com"
            }
        ];

        $scope.find = function (q) {
            // TODO: Implement method.
            alert("artistController.find('" + q + "')");
        }

        $scope.delete = function (id) {
            alert("artistController.delete(" + id + ")");

            // TODO: Implement method.
        }

        $scope.create = function (a) {
            // TODO: Implement method.

            a.id = 0;
            a.name.text = a.name.given[0] + " " + a.name.family[0];
            $scope.artists.push(a);

            // TODO: This is too hard a reset of the object state as it results in all required-fields being flagged in the UI.
            $scope.artist = null;

            $mdDialog.hide();
        }

    }

})();