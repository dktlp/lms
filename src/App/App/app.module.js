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
    });

    app.config(function ($mdThemingProvider) {
        $mdThemingProvider.theme("default")
          .primaryPalette("blue-grey");
    });

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