(function () {
    "use strict";

    angular.module("app").controller("userController", userController);

    function userController($scope)
    {
        $scope.login = function ()
        {
            alert("login [" + $scope.email + ", *********]");
            location.href = "index.html";
        }

        $scope.logout = function ()
        {
            alert("logout");
        }
    }

})();