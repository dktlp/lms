(function () {
    "use strict";

    angular.module("app").controller("dialogController", dialogController);

    function dialogController($scope, $mdDialog, $route) {
        
        $scope.showDialog = function(event, url)
        {
            $mdDialog.show({
                templateUrl: url,
                parent: angular.element(document.body),
                targetEvent: event,
                clickOutsideToClose: false
            }).then(function() {
                $route.reload();
            });            
        }

        $scope.closeDialog = function () {
            $mdDialog.hide();            
        };
        
    }

})();