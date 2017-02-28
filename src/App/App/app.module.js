(function () {
    "use strict";

    var app = angular.module("app", ["ngMaterial", "ngRoute", "ngMessages"]);
    
    app.config(function($routeProvider) {
        $routeProvider
        .when("/", {
            templateUrl : "dashboard.html"
        })
        .when("/artist_list", {
            templateUrl: "/app/views/artist/artist_list.html"
        })
        .when("/artist_overview", {
            templateUrl: "/app/views/artist/artist_overview.html"
        })
    });

    app.config(function($mdThemingProvider) {
        $mdThemingProvider.theme("default")
          .primaryPalette("blue-grey");
    });

})();