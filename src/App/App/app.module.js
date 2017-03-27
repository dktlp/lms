(function () {
    "use strict";

    var app = angular.module("app", ["ngMaterial", "ngRoute", "ngMessages"]);

    app.config(function ($routeProvider) {
        $routeProvider
        .when("/", {
            templateUrl: "dashboard.html"
        })
        .when("/artist_list", {
            templateUrl: "/app/views/artist/artist_list.html"
        })
        .when("/artist_overview", {
            templateUrl: "/app/views/artist/artist_overview.html"
        })
        .when("/label_list", {
            templateUrl: "/app/views/label/label_list.html"
        })
        .when("/accounting_overview", {
            templateUrl: "/app/views/accounting/accounting_overview.html"
        })
    });

    app.config(function ($mdThemingProvider) {
        $mdThemingProvider.theme("default")
          .primaryPalette("blue-grey");
    });

    // TODO: Download ngcookies and add dependency to "ngCookies".

    //app.directive('consent', function ($cookies) {
    //    return {
    //        scope: {},
    //        template:
    //          '<div style="position: relative; z-index: 1000">' +
    //          '<div style="background: #ccc; position: fixed; bottom: 0; left: 0; right: 0" ng-hide="consent()">' +
    //          ' <a href="" ng-click="consent(true)">I\'m cookie consent</a>' +
    //          '</div>' +
    //          '</div>',
    //        controller: function ($scope) {
    //            var _consent = $cookies.get('consent');
    //            $scope.consent = function (consent) {
    //                if (consent === undefined) {
    //                    return _consent;
    //                } else if (consent) {
    //                    $cookies.put('consent', true);
    //                    _consent = true;
    //                }
    //            };
    //        }
    //    };
    //});

    app.factory("contextManagerService", function () {
        var context = [];

        return {
            get: function (name) {
                for (var i = 0; i < context.length; i++) {
                    if (context[i].name == name)
                        return context[i].value;
                }
                return null;
            },
            set: function (name, value) {
                var found = false;
                for (var i = 0; i < context.length; i++) {
                    if (context[i].name == name) {
                        context[i].value = value;
                        found = true;
                    }
                }
                if (!found)
                    context.push({ "name" : name, "value" : value});
            }
        };
    });

})();