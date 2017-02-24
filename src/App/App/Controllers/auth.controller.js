(function () {
    "use strict";

    angular.module("app").controller("authController", authController);

    function authController($scope, $http)
    {
        $scope.login = function ()
        {
            window.sessionStorage.clear();

            var request = httpRequestBuilder("POST", "/api/auth/login", $scope.user);
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
                window.sessionStorage.setItem("tenant", token.tenant);

                window.location.href = "index.html?z=" + session;

            }, function (response)
            {
                if (response.status == 401)
                    $scope.loginForm.$error.auth = true;
                else
                    httpErrorHandler(response);
            });
        }

        $scope.logout = function ()
        {
            window.sessionStorage.clear();
            window.location.href = "login.html";
        }
    }

})();