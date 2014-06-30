﻿'use strict';

// Declare app level module which depends on filters, and services
angular.module('slouchApp', [
  'ngRoute',
  'slouchApp.filters',
  'slouchApp.services',
  'slouchApp.directives',
  'slouchApp.controllers'
]).
config(['$routeProvider', function ($routeProvider) {
    //$routeProvider.when('/view1', { templateUrl: 'Content/partials/partial1.html', controller: 'MyCtrl1' });
    //$routeProvider.when('/view2', { templateUrl: 'Content/partials/partial2.html', controller: 'MyCtrl2' });
    //$routeProvider.otherwise({ redirectTo: '/view1' });
}]);