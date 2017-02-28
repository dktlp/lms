(function () {
    "use strict";

    angular.module("app").controller("menuController", menuController);

    function menuController($scope, $mdDialog) {

        $scope.openMenu = function ($mdOpenMenu, event)
        {
            $mdOpenMenu(event);
        };

    }

})();