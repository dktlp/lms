(function () {
    "use strict";

    angular.module("app").controller("authController", authController);

    function authController($scope, $http)
    {
        $scope.login = function ()
        {
            var request = {
                method: "POST",
                url: "http://localhost:50231/api/auth/login",
                headers: {
                    "Content-Type": "application/json",
                    "lms.tenant.identifier": "0"
                },
                data: $scope.user
            }

            $http(request).then(function (response)
            {
                var base64Url = response.data.split('.')[1];
                var base64 = base64Url.replace('-', '+').replace('_', '/');
                var token = JSON.parse(window.atob(base64));

                var session = "";
                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

                for (var i = 0; i < 32; i++)
                    session += chars.charAt(Math.floor(Math.random() * chars.length));

                window.sessionStorage.setItem("session-id", session);
                window.sessionStorage.setItem("auth-token", response.data);
                window.sessionStorage.setItem("token", token);

                window.location.href = "index.html?z=" + session;

            }, function (response)
            {
                $scope.loginForm.$error.auth = true;
            });
        }

        $scope.logout = function ()
        {
            window.sessionStorage.clear();
            window.location.href = "login.html";
        }
    }

})();