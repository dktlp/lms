(function () {
    "use strict";

    var app = angular.module("app", ["ngMaterial", "ngRoute"]);
    
    app.config(function($routeProvider) {
        $routeProvider
        .when("/", {
            templateUrl : "dashboard.html"
        })
        .when("/artists", {
            templateUrl : "artists.html"
        })
        .when("/blue", {
            templateUrl : "blue.html"
        })
    });

    app.config(function($mdThemingProvider) {
        $mdThemingProvider.theme("default")
          .primaryPalette("blue-grey");
    });

})();