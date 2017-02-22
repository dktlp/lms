(function () {
    "use strict";

    angular.module("app").controller("dialogController", dialogController);

    function dialogController($scope, $mdDialog) {
        
        $scope.showDialog = function(id)
        {
            $mdDialog.show({
                contentElement: "#" + id
            });
        }

        $scope.closeDialog = function () {
            $mdDialog.hide();
        };
        
    }

})();