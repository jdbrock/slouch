'use strict';

/* Controllers */
angular.module('slouchApp.controllers', [])
  .controller('mainController', ['$scope', '$http', function ($scope, $http) {

      //$scope.stuff = 'whit';

      $http({
          method: 'GET',
          url: '/api/status/'
      }).success(function (data, status) {

          $scope.downloads = data.Downloads;

      }).error(function (data, status) {
          
          alert('it broke');

      });

  }]);