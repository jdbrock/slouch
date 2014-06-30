'use strict';

/* Directives */
angular.module('slouchApp.directives', []).
  directive('appVersion', ['version', function (version) {
      return function (scope, elm, attrs) {
          elm.text(version);
      };
  }]);